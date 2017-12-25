using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Globalization;
using FluentFTP;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// FTP 客户端操作工具
    /// </summary>
    public class FTPClientUtility
    {
        #region 构造方法

        public FTPClientUtility(string address, string name, string password, int port = 21, string directory = "/", FTPClientConnecType connecType = FTPClientConnecType.默认) : this(new FTPClientModel()
        {
            Address = address,
            Name = name,
            Password = password,
            Port = port,
            Directory = directory,
            ConnecType = connecType
        })
        {
        }

        public FTPClientUtility(FTPClientModel model)
        {
            IsConnection = false;
            ClientModel = null;
            ClientModel = model;
        }

        #endregion

        #region 属性

        /// <summary>
        /// FTP信息
        /// </summary>
        public FTPClientModel ClientModel { get; private set; }

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnection { get; private set; }

        #endregion

        #region 连接操作

        public void Open()
        {
            IsConnection = false;

            #region 连接信息判断

            if (ClientModel == null)
            {
                throw new ArgumentException("FTP 连接信息为空");
            }

            if (string.IsNullOrWhiteSpace(ClientModel.Address) ||
                string.IsNullOrWhiteSpace(ClientModel.Name) ||
                string.IsNullOrWhiteSpace(ClientModel.Password) ||
                ClientModel.Port == 0)
            {
                throw new ArgumentException("FTP 连接信息为错误(Address/Name/Password,Port)");
            }

            #endregion

            //连接信息
            FtpWebRequest request = GetFTPRequest(ClientModel.Directory);
            if (request == null)
            {
                throw new ArgumentException("FTP Request is null");
            }

            try
            {
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    IsConnection = true;
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Close()
        {
            IsConnection = false;
        }

        #endregion

        #region 上传文件

        /// <summary>
        /// 上传文件（分割上传）
        /// </summary>
        public void Upload(string sourceFilePath, string serverFilePath, int splitSize, Action<long> action)
        {
            //本地文件->流
            FileStream fStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader bReader = new BinaryReader(fStream);

            try
            {
                //处理目录
                var dicPath = Path.GetDirectoryName(serverFilePath).Replace("\\", "/");
                if (dicPath != "/") {
                    DirectoryMake(dicPath);
                }

                //文件大小
                long length = fStream.Length;

                //分割大小
                int byteCount = splitSize * 1024; //200; 500KB

                if (byteCount > length)
                {
                    byteCount = (int)length;
                }

                //开始位置,上传总大小
                long current = 0;

                do
                {
                    //续传处理
                    byte[] data;

                    //己上传,继传,从服务器FTP路径获取文件大小
                    current = GetServerFileSize(serverFilePath);

                    FtpWebRequest ftp = GetFTPRequest(serverFilePath);
                    ftp.ContentLength = length;
                    ftp.Method = WebRequestMethods.Ftp.UploadFile;
                    if (current > 0)
                    {
                        ftp.Method = WebRequestMethods.Ftp.AppendFile;
                        fStream.Seek(current, SeekOrigin.Current);
                    }

                    //把上传的文件写入流
                    using (Stream rs = ftp.GetRequestStream())
                    {
                        for (; current <= length; current = current + byteCount)
                        {
                            long _readLength = 0;

                            if (current + byteCount > length)
                            {
                                _readLength = length - current;
                                data = new byte[_readLength];
                                bReader.Read(data, 0, Convert.ToInt32(_readLength));
                            }
                            else
                            {
                                _readLength = byteCount;
                                data = new byte[byteCount];
                                bReader.Read(data, 0, Convert.ToInt32(_readLength));
                            }
                            rs.Write(data, 0, Convert.ToInt32(_readLength));

                            if (action != null)
                            {
                                action(current + Convert.ToInt32(_readLength));
                            }

                            //文件大小为0，服务器创建文件就OK了
                            //(不跳出的话及while不判断的话 current <= length 为 0=0 无限循环)
                            if (length == 0)
                            {
                                break;
                            }
                        }
                    }
                }
                while (current < length && length > 0);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                bReader.Close();
                fStream.Close();
                GC.Collect();
            }
        }

        #endregion

        #region 信息获取

        /// <summary>
        /// 获取文件大小(字节)
        /// </summary>
        public long GetServerFileSize(string path)
        {
            FtpWebResponse response = null;
            Stream stream = null;

            long size = 0;
            try
            {
                FtpWebRequest request = GetFTPRequest(path);
                request.Method = WebRequestMethods.Ftp.GetFileSize;
                using (response = (FtpWebResponse)request.GetResponse())
                {
                    using (stream = response.GetResponseStream())
                    {
                        size = response.ContentLength;
                        stream.Close();
                    }
                    response.Close();
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (response != null && response.StatusCode == FtpStatusCode.ConnectionClosed)
                {
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return size;
        }

        /// <summary>
        /// 获取路径内容列表
        /// </summary>
        public List<FileStruct> GetServerList(string path)
        {
            List<FileStruct> list = new List<FileStruct>();
            #region FTP 目录内容列表获取(原方式，己注释，linux文件信息解析有问题)

            //List<FileStruct> list = new List<FileStruct>();
            //string[] dataArray = new string[] { };

            //FtpWebResponse response = null;
            //StreamReader stream = null;

            //try
            //{
            //    FtpWebRequest request = GetFTPRequest(path);
            //    request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            //    using (response = (FtpWebResponse)request.GetResponse())
            //    {
            //        using (stream = new StreamReader(response.GetResponseStream()))
            //        {
            //            string dataStr = stream.ReadToEnd();
            //            if (!string.IsNullOrWhiteSpace(dataStr))
            //            {
            //                dataArray = dataStr.Split('\n');
            //            }
            //            stream.Close();
            //        }
            //        response.Close();
            //    }
            //}
            //catch (Exception ex) { }
            //finally
            //{
            //    if (response != null && response.StatusCode == FtpStatusCode.ConnectionClosed)
            //    {
            //        response.Close();
            //    }
            //    if (stream != null)
            //    {
            //        stream.Close();
            //    }
            //}

            //#endregion

            //#region FTP 目录内容处理

            //FileListStyle _directoryListStyle = GuessFileListStyle(dataArray);
            //foreach (string s in dataArray)
            //{
            //    if (_directoryListStyle != FileListStyle.Unknown && s != "")
            //    {
            //        FileStruct f = new FileStruct();
            //        f.Name = "..";
            //        switch (_directoryListStyle)
            //        {
            //            case FileListStyle.UnixStyle:
            //                f = ParseFileStructFromUnixStyleRecord(s);
            //                break;
            //            case FileListStyle.WindowsStyle:
            //                f = ParseFileStructFromWindowsStyleRecord(s);
            //                break;
            //        }
            //        if (!(f.Name == "." || f.Name == ".."))
            //        {
            //            list.Add(f);
            //        }
            //    }
            //}

            #endregion

            FtpClient client = new FtpClient(ClientModel.Address, ClientModel.Port, ClientModel.Name, ClientModel.Password);
            client.Connect();
            foreach (FtpListItem item in client.GetListing(path))
            {
                FileStruct struce = new FileStruct();
                struce.Name = item.Name;
                struce.IsDirectory = item.Type != FtpFileSystemObjectType.File;
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    struce.Size = client.GetFileSize(item.FullName);
                }
                //Flags;
                //Owner;  //item.OwnerPermissions
                //Group;  //item.GroupPermissions
                struce.CreateTime= client.GetModifiedTime(item.FullName);
                list.Add(struce);
            }

            return list;
        }

        /// <summary>
        /// 获取路径文件夹列表
        /// </summary>
        public List<FileStruct> GetServerDirectorys(string path)
        {
            return GetServerList(path).Where(x => x.IsDirectory).ToList();
        }

        /// <summary>
        /// 获取路径文件列表
        /// </summary>
        public List<FileStruct> GetServerFiles(string path)
        {
            return GetServerList(path).Where(x => !x.IsDirectory).ToList();
        }

        #endregion

        #region FTP操作

        /// <summary>
        /// 创建目录
        /// </summary>
        public bool DirectoryMake(string path)
        {
            FtpWebResponse response = null;
            Stream stream = null;

            try
            {
                FtpWebRequest request = GetFTPRequest(path);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (response = (FtpWebResponse)request.GetResponse())
                {
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (response != null && response.StatusCode == FtpStatusCode.ConnectionClosed)
                {
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }

        /// <summary>
        /// 移除目录
        /// </summary>
        public bool DirectoryRemove(string path)
        {
            FtpWebResponse response = null;
            Stream stream = null;

            try
            {
                FtpWebRequest request = GetFTPRequest(path);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                using (response = (FtpWebResponse)request.GetResponse())
                {
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (response != null && response.StatusCode == FtpStatusCode.ConnectionClosed)
                {
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }

        /// <summary>
        /// 重名操作
        /// </summary>
        public bool Rename(string path, string newName)
        {
            FtpWebResponse response = null;
            Stream stream = null;

            try
            {
                FtpWebRequest request = GetFTPRequest(path);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = newName;
                using (response = (FtpWebResponse)request.GetResponse())
                {
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (response != null && response.StatusCode == FtpStatusCode.ConnectionClosed)
                {
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取FTP请求信息
        /// </summary>
        private FtpWebRequest GetFTPRequest(string path)
        {
            FtpWebRequest request = null;
            try
            {
                string currentDirectory = path.TrimStart('/');
                if (!string.IsNullOrWhiteSpace(currentDirectory))
                {
                    currentDirectory = "/" + currentDirectory;
                }

                //根据服务器信息FtpWebRequest创建类的对象
                string uriStr = string.Format("ftp://{0}:{1}{2}", ClientModel.Address, ClientModel.Port, currentDirectory);
                request = (FtpWebRequest)FtpWebRequest.Create(uriStr);

                //提供身份验证信息
                request.Credentials = new System.Net.NetworkCredential(ClientModel.Name, ClientModel.Password);

                ////指定文件传输的数据类型
                request.UseBinary = true;
                request.UsePassive = ClientModel.ConnecType == FTPClientConnecType.主动模式 ? false : true; //true 被动

                //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
                request.KeepAlive = true;
            }
            catch (Exception ex) { throw ex; }

            return request;
        }

        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        private FileListStyle GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10
                 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyle.UnixStyle;
                }
                else if (s.Length > 8
                 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyle.WindowsStyle;
                }
            }
            return FileListStyle.Unknown;
        }

        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string record)
        {
            FileStruct f = new FileStruct();
            string processstr = record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }

        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        private FileStruct ParseFileStructFromUnixStyleRecord(string record)
        {
            FileStruct f = new FileStruct();
            string processstr = record.Trim(), sizeStr = record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }

            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            if (yearOrTime.IndexOf(":") >= 0)  //time
            {
                f.CreateTime = DateTime.Parse(f.CreateTime.ToShortDateString() + " " + yearOrTime);
            }

            //Size处理
            f.Size = 0;
            sizeStr = sizeStr.IndexOf("   ") > -1 ? sizeStr.Substring(sizeStr.IndexOf("   ")).Trim() : "";
            if (!string.IsNullOrWhiteSpace(sizeStr))
            {
                f.Size = LongHelper.Get(sizeStr.IndexOf(" ") > -1 ? sizeStr.Substring(0, sizeStr.IndexOf(" ")) : "");
            }

            f.Name = processstr;   //最后就是名称
            return f;
        }

        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }

        #endregion
    }

    #region FTP信息结构

    /// <summary>
    /// FTP 客户端 信息实体
    /// </summary>
    public class FTPClientModel
    {
        /// <summary>
        /// 连接地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 用户名 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 目录 
        /// </summary>
        public string Directory { get { return directory; } set { directory = value; } }
        private string directory = "/";

        /// <summary>
        /// 连接模式（主/被(默认) 动）
        /// </summary>
        public FTPClientConnecType ConnecType { get { return connecType; } set { connecType = value; } }
        private FTPClientConnecType connecType = FTPClientConnecType.默认;
    }

    /// <summary>
    /// FTP客户端连接模式(默认为被动)
    /// </summary>
    public enum FTPClientConnecType
    {
        默认 = 0,
        主动模式 = 1,
        被动模式 = 2
    }

    #endregion

    #region 文件信息结构
    public struct FileStruct
    {
        public string Name;
        public bool IsDirectory;
        public long Size;
        public string Flags;
        public string Owner;
        public string Group;
        public DateTime CreateTime;
    }

    public enum FileListStyle
    {
        UnixStyle,
        WindowsStyle,
        Unknown
    }
    #endregion


}
