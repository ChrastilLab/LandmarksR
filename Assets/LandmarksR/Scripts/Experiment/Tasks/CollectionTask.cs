using System.Collections;
using System.Collections.Generic;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public enum NodeMoveDirection
    {
        Next,
        Previous,
    }

    public class CollectionTask : BaseTask
    {
        private LinkedList<BaseTask> _taskList;
        private LinkedListNode<BaseTask> _currentNode;
        private NodeMoveDirection _nodeMoveDirection = NodeMoveDirection.Next;

        protected override void Prepare()
        {
            base.Prepare();
            isRunning = false;
            _taskList = new LinkedList<BaseTask>(subTasks);
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

        public void SkipNext()
        {
            if (_currentNode.Next != null)
            {
                _currentNode = _currentNode.Next;
            }
        }

        public void MoveToPrevious()
        {
            _nodeMoveDirection = NodeMoveDirection.Previous;
        }

        protected void ResetNode()
        {
            _currentNode = _taskList.First;
        }
    }
}
