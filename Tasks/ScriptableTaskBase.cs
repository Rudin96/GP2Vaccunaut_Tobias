using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team14
{
    [Serializable]
    public enum TaskStatus
    {
        Inactive,
        InProgress,
        Completed
    }
    public abstract class ScriptableTaskBase : ScriptableObject
    {
        private event Action _response;
        private event Action _finishResponse;

        public string Description;
        public TaskStatus TaskStatus;
        public abstract void Update();
        public abstract void ResetCounter();
        public abstract string GetProgressText();
        protected void Finish()
        {
            TaskStatus = TaskStatus.Completed;
            _finishResponse?.Invoke();
            RaiseUI();
        }

        protected void RaiseUI()
        {
            _response?.Invoke();
        }

        public void SetActive()
        {
            TaskStatus = TaskStatus.InProgress;
            RaiseUI();
        }

        public void SetInactive()
        {
            TaskStatus = TaskStatus.Inactive;
            RaiseUI();
        }

        public void FinishRegister(Action response)
        {
            _finishResponse += response;
        }

        public void FinishUnRegister(Action response)
        {
            _finishResponse -= response;
        }

        public void Register(Action response)
        {
            _response += response;
        }

        public void UnRegister(Action response)
        {
            _response -= response;
        }

        public void RegisterActive(Action response)
        {
            _response += response;
        }

        public void UnregisterActive(Action response)
        {
            _response -= response;
        }
    }
}