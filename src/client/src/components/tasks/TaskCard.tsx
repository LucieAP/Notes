import { GetTaskResponse } from "@/features/tasks/task";
import TaskIcon from "../common/icons/TaskIcon";

interface Props {
  task: GetTaskResponse;
}

function TaskCard({ task }: Props) {
  return (
    <div className="flex space-x-1 p-2 hover:bg-neutral-600 rounded-lg">
      <TaskIcon />
      <span className="select-none underline decoration-neutral-600 underline-offset-3">
        {task.title}
      </span>
    </div>
  );
}

export default TaskCard;

