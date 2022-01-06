using System.Collections.Generic;
using DefaultNamespace.Infrastructure.Data.ParstOfCardProfiles;
using UnityEngine;

namespace DefaultNamespace.Infrastructure.Data
{
    public class ObjectCardProfile<T, TData> : ScriptableObject where T : Object where TData : SaveDataProfile
    {
        public T Target => _target;
        [SerializeField] protected T _target;
        [SerializeField] private TData _defaultData;
        [SerializeField] protected List<PartOfCardProfile> _parts;

        public TData CurrentData => _currentData;
        private TData _currentData;
        
        [SerializeField] private bool _setDefault;

        [ContextMenu("SetToDefault")]
        private void SetDefault()
        {
            var ser = new BinaryProvider();
            SaveNewData(ser, _defaultData);
        }
        
        public void UpdateData(SaveDataProvider IProvider)
        {
            var currentData = IProvider.GetOrNull<ObjectCardProfile<T, TData>, T, TData>(_target);
            if (currentData == null)
            {
                currentData = SaveNewData(IProvider, _defaultData);
            }
            _currentData = currentData;
        }

        public TData SaveNewData(SaveDataProvider IProvider, TData newData)
        {
            IProvider.Save<ObjectCardProfile<T, TData>, T, TData>(_target, newData);
            return _currentData=IProvider.GetOrNull<ObjectCardProfile<T, TData>, T, TData>(_target);
        }

        public Y GetFirstOrNull<Y>() where Y : PartOfCardProfile
        {
            foreach (var partOfCardProfile in _parts)
            {
                if (partOfCardProfile.GetType() == typeof(Y))
                    return partOfCardProfile as Y;
            }

            return null;
        }

        public List<Y> GetAll<Y>() where Y : PartOfCardProfile
        {
            List<Y> results = new List<Y>();
            _parts.ForEach(x =>
            {
                if(typeof(Y) == x.GetType())
                    results.Add(x as Y);
            });
            return results;
        }

        public void OnValidate()
        {
            if(_target==null)
                Debug.LogWarning($"{name} не имеет целевого объекта для карточки, FIX MEEEEEEEEEEEEEEEE");
            if(_setDefault)
                SetDefault();
            CustomOnValidate();
        }
        
        protected virtual void CustomOnValidate(){}
    }
}