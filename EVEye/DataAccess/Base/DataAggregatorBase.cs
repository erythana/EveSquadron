using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVEye.DataAccess.Interfaces;

namespace EVEye.DataAccess.Base
{
    public abstract class DataAggregatorBase<T, TInput>
    {
        protected DataAggregatorBase()//TODO: ISQLLite cache?
        {
        }

        #region interface implementation

        protected abstract Task<IEnumerable<T>> GetAggregatedItems(TInput input);

        #endregion
    }

}