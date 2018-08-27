using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Unidal.Cat
{
    public class TransactionUtil
    {
        public static async Task<TResult> WrapWithForkedTransactionAsync<TResult>(string funcName, Func<Task<TResult>> func)
        {
            if (!Cat.Enabled)
                return await func.Invoke();

            var forkedTran = Org.Unidal.Cat.Cat.NewForkedTransaction("remote", funcName);
            try
            {
                var result = await func.Invoke();
                forkedTran.Status = Org.Unidal.Cat.CatConstants.SUCCESS;

                return result;
            }
            catch (Exception ex)
            {
                forkedTran?.SetStatus(ex);
                return default(TResult);
                throw;
            }
            finally
            {
                forkedTran?.Complete();
            }
        }

        public static async Task WrapWithForkedTransactionAsync(string funcName, Func<Task> func)
        {
            if (!Cat.Enabled)
            {
                await func.Invoke();
                return;
            }

            var forkedTran = Org.Unidal.Cat.Cat.NewForkedTransaction("remote", funcName);
            try
            {
                await func.Invoke();
                forkedTran.Status = Org.Unidal.Cat.CatConstants.SUCCESS;
            }
            catch (Exception ex)
            {
                forkedTran?.SetStatus(ex);
                throw;
            }
            finally
            {
                forkedTran?.Complete();
            }
        }
    }
}
