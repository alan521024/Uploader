using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using WinForm = System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DoubleX.Infrastructure.Utility;
using DoubleXUI.Controls;
using System.Security.Cryptography;

namespace DoubleX.Upload
{
    public class AppHelper
    {
        #region 数据库相关

        /// <summary>
        /// 数据库目录
        /// </summary>
        public static string DatabasePath { get { return @"Data\"; } }

        /// <summary>
        /// 任务数据库
        /// </summary>
        /// <returns></returns>
        public static string GetTaskDatabaseConnectionStr()
        {
            return string.Format(@"Data Source={0}database.db;", DatabasePath);
        }


        /// <summary>
        /// 任务文件数据库
        /// </summary>
        /// <param name="destDbPath"></param>
        /// <returns></returns>
        public static string GetTaskFileDatabaseConnectionStr(string destDbPath, string rootPath = null)
        {
            if (!string.IsNullOrWhiteSpace(rootPath))
            {
                destDbPath = string.Format("{0}{1}", rootPath, destDbPath);
            }
            return string.Format(@"Data Source={0};", destDbPath);
        }

        #endregion

        #region DataRow 转实体

        /// <summary>
        /// 获取任务信息实体
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static TaskEntity GetTaskEntityByRow(DataRow row)
        {
            TaskEntity model = null;
            if (row != null)
            {
                model = new TaskEntity();
                model.Id = StringHelper.Get(row["Id"]);
                model.TaskName = StringHelper.Get(row["TaskName"]);
                model.PathJSON = StringHelper.Get(row["PathJSON"]);
                model.BeforeJSON = StringHelper.Get(row["BeforeJSON"]);
                model.AfterJSON = StringHelper.Get(row["AfterJSON"]);
                model.SettingJSON = StringHelper.Get(row["SettingJSON"]);
                model.FileTotal = (double)LongHelper.Get(row["FileTotal"]);
                model.SuccessTotal = (double)LongHelper.Get(row["SuccessTotal"]);
                model.ErrorTotal = (double)LongHelper.Get(row["ErrorTotal"]);
                model.ErrorJSON = StringHelper.Get(row["ErrorJSON"]);
                model.Status = IntHelper.Get(row["Status"]);
                model.CreateDt = DateTimeHelper.Get(row["CreateDt"]);

            }
            return model;
        }

        /// <summary>
        /// 获取上传文件信息实体
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static TaskFileEntity GetTaskFileEntityByRow(DataRow row)
        {
            TaskFileEntity model = null;
            if (row != null)
            {
                model = new TaskFileEntity();
                model.Id = StringHelper.Get(row["Id"]);
                model.TaskId = StringHelper.Get(row["TaskId"]);
                model.FilePath = StringHelper.Get(row["FilePath"]);
                model.FileSize = LongHelper.Get(row["FileSize"]);
                model.FileName = StringHelper.Get(row["FileName"]);
                model.FileExtension = StringHelper.Get(row["FileExtension"]);
                model.ServerFullPath = StringHelper.Get(row["ServerFullPath"]);
                model.UpSize = LongHelper.Get(row["UpSize"]);
                model.BeforeResult = StringHelper.Get(row["BeforeResult"]);
                model.Status = IntHelper.Get(row["Status"]);
                model.LastDt = DateTimeHelper.Get(row["LastDt"]);

                if (string.IsNullOrWhiteSpace(model.ServerFullPath))
                {
                    model.ServerFullPath = model.FileName;
                }
            }
            return model;
        }

        #endregion

        #region 配置文件

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static ConfigModel GetConfig()
        {
            string cacheKey = "_cache";

            RuntimeCaching cachingHelper = new RuntimeCaching();

            ConfigModel configModel = cachingHelper.Get(cacheKey) as ConfigModel;
            if (configModel != null)
            {
                return configModel;
            }


            var configPath = string.Format("{0}/data/config.xml", AppDomain.CurrentDomain.BaseDirectory).ToLower();
            if (File.Exists(configPath))
            {
                configModel = XmlHelper.Load(typeof(ConfigModel), configPath) as ConfigModel;
            }

            if (configModel == null)
            {
                configModel = new ConfigModel() { VersionUrl = "#" };
            }

            cachingHelper.Set<ConfigModel>(cacheKey, configModel);

            return configModel;
        }

        #endregion

        #region 授权文件/注册表 

        /// <summary>
        ///授权文件获取
        /// </summary>
        public static LicenseFileModel LicenseFileGet(string path)
        {
            if (!File.Exists(path))
            {
                throw new LicenseException(LicenseExceptionType.授权文件不存在);
            }

            //授权信息
            LicenseFileModel model = null;

            //公钥路径
            var pubKeyPath = string.Format("{0}/data/doublex.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();

            //字符编号
            UTF8Encoding enc = new UTF8Encoding();

            //读取注册数据文件
            StreamReader sr = new StreamReader(path, UTF8Encoding.UTF8);
            string encrytText = sr.ReadToEnd();
            sr.Close();
            byte[] encrytBytes = System.Convert.FromBase64CharArray(encrytText.ToCharArray(), 0, encrytText.Length);

            //读取公钥
            StreamReader srPublickey = new StreamReader(pubKeyPath, UTF8Encoding.UTF8);
            string publicKey = srPublickey.ReadToEnd();
            srPublickey.Close();

            //用公钥初化始RSACryptoServiceProvider类实例crypt。
            RSACryptoServiceProvider crypt = new RSACryptoServiceProvider();
            crypt.FromXmlString(AESHelper.AESDecrypt(publicKey));

            int keySize = crypt.KeySize / 8;
            byte[] buffer = new byte[keySize];
            MemoryStream msInput = new MemoryStream(encrytBytes);
            MemoryStream msOuput = new MemoryStream();
            int readLen = msInput.Read(buffer, 0, keySize);
            while (readLen > 0)
            {
                byte[] dataToDec = new byte[readLen];
                Array.Copy(buffer, 0, dataToDec, 0, readLen);
                byte[] decData = crypt.Decrypt(dataToDec, false);
                msOuput.Write(decData, 0, decData.Length);
                readLen = msInput.Read(buffer, 0, keySize);
            }

            msInput.Close();
            byte[] result = msOuput.ToArray();    //得到解密结果
            msOuput.Close();
            crypt.Clear();

            string decryptText = enc.GetString(result);
            try
            {
                if (!string.IsNullOrWhiteSpace(decryptText))
                {
                    model = JsonHelper.Deserialize<LicenseFileModel>(decryptText);
                }
            }
            catch (Exception ex)
            {
                throw new LicenseException(LicenseExceptionType.授权文件内容错误);
            };
            return model;
        }

        /// <summary>
        /// 授权统计获取
        /// </summary>
        /// <param name="isAddSelf"></param>
        /// <returns></returns>
        public static LicenseStatModel LicenseStatGet(LicenseFileModel licenseFileModel, bool isAddSelf = true)
        {
            if (licenseFileModel == null)
                throw new LicenseException(LicenseExceptionType.授权信息错误);

            if (VerifyHelper.IsEmpty(licenseFileModel.Email))
                throw new LicenseException(LicenseExceptionType.授权信息错误);

            if (VerifyHelper.IsEmpty(licenseFileModel.Mobile))
                throw new LicenseException(LicenseExceptionType.授权信息错误);
            

            LicenseStatModel model = new LicenseStatModel();

            RegisterUtil reg = new RegisterUtil("software\\DxUpload\\");
            if (!reg.IsSubKeyExist())
            {
                reg.CreateSubKey("software\\DxUpload\\");
            }

            object identificationItem = reg.ReadRegeditKey("identification");
            if (identificationItem == null || (!VerifyHelper.IsEmpty(identificationItem) 
                && StringHelper.Get(identificationItem).ToLower()!= licenseFileModel.Email.ToLower()))
            {
                model.Identification = licenseFileModel.Email;
                reg.WriteRegeditKey("identification", licenseFileModel.Email.ToLower());
            }
            else
            {
                model.Identification = StringHelper.Get(identificationItem);
            }

            object mobileItem = reg.ReadRegeditKey("mobile");
            if (mobileItem == null || (!VerifyHelper.IsEmpty(mobileItem)
                && StringHelper.Get(mobileItem).ToLower() != licenseFileModel.Mobile.ToLower()))
            {
                model.Mobile = licenseFileModel.Mobile;
                reg.WriteRegeditKey("mobile", licenseFileModel.Mobile.ToLower());
            }
            else
            {
                model.Mobile = StringHelper.Get(mobileItem);
            }


            object countItem = reg.ReadRegeditKey("count");
            if (countItem == null)
            {
                model.Count = 0;
            }
            else
            {
                model.Count = LongHelper.Get(countItem);
            }

            if (isAddSelf)
            {
                model.Count = model.Count + 1;
                reg.WriteRegeditKey("count", model.Count);
            }

            object creatItem = reg.ReadRegeditKey("creat");
            if (creatItem == null)
            {
                model.Create = DateTime.Now;
                reg.WriteRegeditKey("creat", model.Create);
            }
            else
            {
                model.Create = DateTimeHelper.Get(creatItem);
            }

            model.Mac = MacHelper.GetMacAddress();
            model.Cpu = Win32Helper.GetCpuID();

            return model;
        }

        /// <summary>
        /// 授权统计重置
        /// </summary>
        public static void LicenseStatReset(LicenseFileModel licenseFileModel)
        {
            if (licenseFileModel == null)
                throw new LicenseException(LicenseExceptionType.授权信息错误);

            if (VerifyHelper.IsEmpty(licenseFileModel.Email))
                throw new LicenseException(LicenseExceptionType.授权信息错误);

            if (VerifyHelper.IsEmpty(licenseFileModel.Mobile))
                throw new LicenseException(LicenseExceptionType.授权信息错误);

            RegisterUtil reg = new RegisterUtil("software\\DxUpload\\");
            if (!reg.IsSubKeyExist())
            {
                reg.CreateSubKey("software\\DxUpload\\");
            }

            reg.WriteRegeditKey("identification", licenseFileModel.Email.ToLower());
            reg.WriteRegeditKey("mobile", licenseFileModel.Mobile.ToLower());
            reg.WriteRegeditKey("count", 1);
            reg.WriteRegeditKey("creat", DateTime.Now);
        }

        /// <summary>
        /// 授权文件验证
        /// </summary>
        public static bool LicenseVerify(LicenseFileModel fileModel, LicenseStatModel statModel)
        {
            //数据验证
            if (fileModel == null || statModel == null)
            {
                throw new LicenseException(LicenseExceptionType.授权文件内容错误);
            }

            //产品验证
            if (fileModel.Product.ToLower() != "doublex.upload")
            {
                throw new LicenseException(LicenseExceptionType.授权产品错误);
            }

            //版本标识
            string basicTag = EnumEditionType.Basic.ToString().ToLower(), professionalTag = EnumEditionType.Professional.ToString().ToLower();
            var currentEdition = fileModel.Edition.ToLower();

            //版本验证，基础/专业版
            if (!(currentEdition == basicTag || currentEdition == professionalTag))
            {
                throw new LicenseException(LicenseExceptionType.授权版本错误);
            }

            //试用版数据验证
            if (fileModel.IsTrial)
            {
                //是否试用授权文件
                if (!(fileModel.Email.ToLower() == "demo@demo.com" && fileModel.Mac.ToLower() == "xx-xx-xx-xx-xx-xx" && fileModel.Cpu.ToLower() == "xxxxxxxxxxxxxxxx" && fileModel.Mobile.ToLower() == "xxxxxxxxxxx"))
                {
                    throw new LicenseException(LicenseExceptionType.授权试用错误);
                }

                //是否可继续试用(判断时间及次数)
                var maxCount = LongHelper.Get(fileModel.Times);
                if (maxCount > 0 && statModel.Count > maxCount)
                {
                    throw new LicenseException(LicenseExceptionType.授权试用次数超出);
                }

                //可以通过http获取在线时间（防止更改时间）
                var curDate = DateTime.Now;
                var minDate = DateTimeHelper.Get("1900-01-01 00:00");

                //时间格式：201x-xx-xx（指定日期过期）
                var maxDate = DateTimeHelper.Get(fileModel.Date, defaultValue: minDate);
                if (maxDate > minDate && curDate > DateTimeHelper.GetEnd(maxDate))
                {
                    throw new LicenseException(LicenseExceptionType.授权试用过期);
                }

                //时间格式：2 (安装后1天过期)
                var maxDate2 = IntHelper.Get(fileModel.Date);
                if (maxDate2 > 0 && curDate > DateTimeHelper.GetEnd(statModel.Create.AddDays(maxDate2)))
                {
                    throw new LicenseException(LicenseExceptionType.授权试用过期);
                }

                //试用分版本(暂不分版本) 
                ////基础版
                //if (currentEdition == basicTag)
                //{
                //}

                ////专业版
                //if (currentEdition == professionalTag)
                //{
                //}
            }
            else
            {
                //基本信息验证
                if (fileModel.Email != statModel.Identification ||
                    fileModel.Mobile != statModel.Mobile ||
                    fileModel.Mac != statModel.Mac ||
                    fileModel.Cpu != statModel.Cpu)
                {
                    throw new LicenseException(LicenseExceptionType.授权信息错误);
                }


                //基础版
                if (currentEdition == basicTag)
                {

                }

                //专业版
                if (currentEdition == professionalTag)
                {

                }
            }

            return true;
        }

        #endregion
    }
}
