using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{

    public enum NodeMoveDirection
    {
        None,
        Next,
        Previous,
    }
    public class CollectionTask : BaseTask
    {
        private LinkedList<BaseTask> _taskList;
        private LinkedListNode<BaseTask> _currentNode;
        private NodeMoveDirection _nodeMoveDirection = NodeMoveDirection.None;
        protected override void Prepare()
        {
            base.Prepare();
            isRunning = false;
            _taskList = new LinkedList<BaseTask>(SubTasks);
        }

        public override IEnumerator ExecuteAll()
        {
            if (!enable) yield break;

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
                        _nodeMoveDirection = NodeMoveDirection.None;
                        break;
                    case NodeMoveDirection.Next when _currentNode.Next != null:
                        _currentNode = _currentNode.Next;
                        _nodeMoveDirection = NodeMoveDirection.None;
                        break;
                    case NodeMoveDirection.None:
                    default:
                        _currentNode = _currentNode.Next;
                        break;
                }
            }

            Finish();
        }

        public void MoveToNext()
        {
            _nodeMoveDirection = NodeMoveDirection.Next;
        }

        public void MoveToPrevious()
        {
            _nodeMoveDirection = NodeMoveDirection.Previous;
        }
    }
}
