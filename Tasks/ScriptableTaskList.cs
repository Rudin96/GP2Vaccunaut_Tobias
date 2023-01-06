using System.Linq;
using System.Collections.Generic;
using Team14;
using UnityEngine;

[CreateAssetMenu(fileName = "new ScriptableTaskList", menuName = "ScriptableTasks/ScriptableTaskList", order = 0)]
public class ScriptableTaskList : ScriptableObject
{
    public List<ScriptableTaskBase> Items;
    public List<ScriptableTaskBase> GetUnfinishedTasks()
    {
        return Items.Where(t => t.TaskStatus == TaskStatus.InProgress).ToList();
    }

    public List<ScriptableTaskBase> GetNotInactiveTasks()
    {
        return Items.Where(t => t.TaskStatus != TaskStatus.Inactive).ToList();
    }

    public List<ScriptableTaskBase> GetFinishedTasks()
    {
        return Items.Where(t => t.TaskStatus == TaskStatus.Completed).ToList();
    }

    public void ResetTasksProgress()
    {
        Items.ForEach(t =>
        {
            t.TaskStatus = TaskStatus.Inactive;
            t.ResetCounter();
        });
    }
}