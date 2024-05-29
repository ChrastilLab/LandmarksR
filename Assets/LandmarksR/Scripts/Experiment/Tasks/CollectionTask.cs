using System.Collections;
using System.Collections.Generic;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    /// <summary>
    /// Enum representing the direction to move between nodes in the task collection.
    /// </summary>
    public enum NodeMoveDirection
    {
        /// <summary>
        /// Move to the next node.
        /// </summary>
        Next,

        /// <summary>
        /// Move to the previous node.
        /// </summary>
        Previous,
    }

    /// <summary>
    /// Represents a collection of tasks that can be executed sequentially.
    /// A LinkedList is used to allow for easy traversal between tasks.
    /// </summary>
    public class CollectionTask : BaseTask
    {
        /// <summary>
        /// The list of tasks to be executed.
        /// </summary>
        private LinkedList<BaseTask> _taskList;

        /// <summary>
        /// The current node in the task list.
        /// </summary>
        private LinkedListNode<BaseTask> _currentNode;

        /// <summary>
        /// The direction to move between nodes.
        /// </summary>
        private NodeMoveDirection _nodeMoveDirection = NodeMoveDirection.Next;

        /// <summary>
        /// Prepares the collection task for execution.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Structural);
            base.Prepare();

            _taskList = new LinkedList<BaseTask>(_subTasks);
        }

        /// <summary>
        /// Executes all tasks in the collection.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        public override IEnumerator ExecuteAll()
        {
            if (!_enable) yield break;

            Prepare();

            isSubTaskRunning = true;
            _currentNode = _taskList.First;
            while (_currentNode != null)
            {
                var task = _currentNode.Value;
                yield return task.ExecuteAll();
                switch (_nodeMoveDirection)
                {
                    case NodeMoveDirection.Previous when _currentNode.Previous != null:
                        _currentNode = _currentNode.Previous;
                        _nodeMoveDirection = NodeMoveDirection.Next;
                        break;
                    case NodeMoveDirection.Next when _currentNode.Next != null:
                    default:
                        _currentNode = _currentNode.Next;
                        break;
                }
            }

            Finish();
        }

        /// <summary>
        /// Skips to the next task in the collection.
        /// </summary>
        public void SkipNext()
        {
            if (_currentNode.Next != null)
            {
                _currentNode = _currentNode.Next;
            }
        }

        /// <summary>
        /// Moves to the previous task in the collection.
        /// </summary>
        public void MoveToPrevious()
        {
            _nodeMoveDirection = NodeMoveDirection.Previous;
        }

        /// <summary>
        /// Resets the current node to the first task in the collection.
        /// </summary>
        protected void ResetNode()
        {
            _currentNode = _taskList.First;
        }
    }
}
