using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Team14
{
    public class TaskManager : MonoBehaviour
    {
        public ScriptableTaskList CurrentTasks;
        public List<LevelTask> LevelTasks;

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            //EditorApplication.update += Validate;
            //CurrentTasks.ResetTasksProgress();
            foreach (var t in LevelTasks)
            {
                if(t.Response != null)
                    t.Task.FinishRegister(t.Response.Invoke);
            }
        }

        private void OnDisable()
        {
            foreach (var t in LevelTasks)
            {
                if(t.Response != null)
                    t.Task.FinishUnRegister(t.Response.Invoke);
            }
        }

        private void OnDrawGizmos()
        {
            AdjustTaskList();
        }

        private void AdjustTaskList()
        {
            if (CurrentTasks != null)
            {
                if (LevelTasks.Count == 0)
                    CurrentTasks.Items.ForEach(t =>
                    {
                        LevelTasks.Add(new LevelTask { Task = t });
                    });
                if (LevelTasks.Count > CurrentTasks.Items.Count)
                {
                    List<LevelTask> list = new List<LevelTask>();
                    LevelTasks.ForEach(t => list.Add(t));
                    LevelTasks.Clear();
                    LevelTasks = list;
                    LevelTasks.RemoveAt(LevelTasks.Count - 1);
                }
                else if (LevelTasks.Count < CurrentTasks.Items.Count)
                {
                    List<LevelTask> list = new List<LevelTask>();
                    LevelTasks.ForEach((t) => list.Add(t));
                    LevelTasks.Clear();
                    LevelTasks = list;
                    LevelTasks.Add(new LevelTask { Task = CurrentTasks.Items[CurrentTasks.Items.Count - 1] });
                }

                if (LevelTasks.Count == CurrentTasks.Items.Count)
                {
                    for (int i = 0; i < LevelTasks.Count; i++)
                    {
                        if (LevelTasks[i].Task != CurrentTasks.Items[i])
                            LevelTasks.Insert(i, new LevelTask { Task = CurrentTasks.Items[i]});
                    }
                }
            }
            else if (CurrentTasks == null)
                LevelTasks.Clear();
            else if (CurrentTasks == null && LevelTasks.Count > 0)
                return;
        }

    }


    [Serializable]
    public class LevelTask
    {
        public ScriptableTaskBase Task;
        public UnityEvent Response;
    }
}