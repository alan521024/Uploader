using System;
using System.Text;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 异常辅助类
    /// </summary>
    public class ExceptionHelper
    {
        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns></returns>
        public static string GetMessage(Exception ex)
        {
            if (VerifyHelper.IsNull(ex))
            {
                return "";
            }
            if (ex is LicenseException)
            {
                LicenseException lsEx = ex as LicenseException;
                if (lsEx != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0}", lsEx.ExceptionType.ToString());
                    if (!string.IsNullOrWhiteSpace(lsEx.Message))
                    {
                        sb.AppendFormat("：{0}", lsEx.Message);
                    }
                    return sb.ToString();
                }
            }
            if (ex is DbEntityValidationException)
            {
                DbEntityValidationException valEx = ex as DbEntityValidationException;
                StringBuilder sb = new StringBuilder();
                foreach (var failure in valEx.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }
                return sb.ToString();
            }
            else if (ex is DbUpdateException)
            {
                DbUpdateException updateEx = ex as DbUpdateException;
                if (updateEx.InnerException != null)
                {
                    return updateEx.InnerException.InnerException != null ? updateEx.InnerException.InnerException.ToString() : updateEx.InnerException.Message;
                }
                return updateEx.Message;
                //StringBuilder sb = new StringBuilder();
                //foreach (var failure in updateEx.)
                //{
                //    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                //    foreach (var error in failure.ValidationErrors)
                //    {
                //        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                //        sb.AppendLine();
                //    }
                //}
                //return sb.ToString();
            }
            else if (ex is DbUpdateConcurrencyException)
            {
                return "框架在更新时引起了乐观并发，后修改的数据不会被保存";
            }

            string message = (ex.InnerException != null && ex.InnerException.Message != null) ?
                    ex.InnerException.Message : ex.Message;

            message = !VerifyHelper.IsEmpty(message) ? message : ex.ToString();
            return message;
        }
    }
}
