using System.Collections.Generic;
using UnityEngine;

namespace _AppAssets.Code
{
    public class MonoBehaviourPool <TPoolable> : MonoBehaviour where TPoolable : MonoBehaviour, IPoolable
    {
        public TPoolable _poolableItem;
        protected int _poolSize;
        private string _poolName;
        private Transform _poolParent;
        private List<TPoolable> _inactiveObjects;
        private List<TPoolable> _activeObjects;
        
        protected void Initialize(int poolSize)
        {
            _poolSize = poolSize;

            if (_poolParent == null)
            {
                _poolName = typeof(TPoolable).Name;
                
                _poolParent = new GameObject($"{_poolName} Pool").transform;
                _poolParent.parent = transform;
            }

            InitializePool();
        }

        protected void ResizePool(int newSize)
        {
            if (newSize <= _poolSize)
            {
                return;
            }

            var diff = newSize - _poolSize;
            _poolSize += diff;
            
            for (int i = 0; i < diff; i++)
            {
                var instance = Instantiate(_poolableItem, _poolParent);
                instance.Reset();
                _inactiveObjects.Add(instance);
            }
        }

        protected TPoolable GetItemFromPool()
        {
            //Check if there are objects in the pool!!!!
            
            var newItem = _inactiveObjects[0];
            _activeObjects.Add(newItem);
            _inactiveObjects.RemoveAt(0);
            
            newItem.gameObject.SetActive(true);

            return newItem;
        }
        
        protected List<TPoolable> GetItemsFromPool(int amount)
        {
            //What if amount > _inactiveObjects.Count ????
            
            List<TPoolable> newItems = new List<TPoolable>(amount);

            for (int i = 0; i < amount; i++)
            {
                var item = _inactiveObjects[i];
                item.OnSendToPool += ReturnItemToPool;
                newItems.Add(item);
            }
            
            _activeObjects.AddRange(newItems);
            _inactiveObjects.RemoveRange(0, amount);

            return newItems;
        }

        protected void ReturnItemToPoolAndReset(IPoolable item)
        {
            ReturnItemToPool(item);
            item.Reset();
        }

        private void ReturnItemToPool(IPoolable item)
        {
            var poolableItem = item as TPoolable;// ?????? Will this crash?
            _activeObjects.Remove(poolableItem); 
            _inactiveObjects.Add(poolableItem);
            poolableItem.transform.parent = _poolParent;
            
            item.OnSendToPool -= ReturnItemToPool;
        }

        protected void ResetPool()
        {
            var activeItems = _activeObjects.Count;
            for(int i = activeItems-1; i>=0; i--)
            {
                ReturnItemToPoolAndReset(_activeObjects[i]);
            }
            
            //Option 2:
            // foreach (var activeItem in _activeObjects)
            // {
            //     activeItem.Reset();
            //     activeItem.OnSendToPool -= ReturnItemToPool;
            //     _inactiveObjects.Add(activeItem);
            // }
            // _activeObjects.Clear();
        }

        private void InitializePool()
        {
            _inactiveObjects = new List<TPoolable>(_poolSize);
            _activeObjects = new List<TPoolable>(_poolSize);

            for (int i = 0; i < _poolSize; i++)
            {
                var instance = Instantiate(_poolableItem, _poolParent);
                instance.Reset();
                _inactiveObjects.Add(instance);
            }
        }
    }
}