using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVEye.DataAccess.Interfaces
{
    public interface IDataAggregator<T>
    {
        public Task<IEnumerable<T>> GetAggregatedItemsFor<U>(U input);
        
        public event EventHandler AggregatedItemsChanged;
    }
}