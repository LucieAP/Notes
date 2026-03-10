import { useLocation } from "react-router-dom";
import { EntityType } from "../types/entityType";

function useActiveEntityType(): EntityType | undefined {
  const { pathname } = useLocation(); // "/notes /tasks ..."
  const entity = pathname.split("/")[1]; // "notes" | "tasks" | "recipes" | ...

  if (entity === "notes" || entity === "tasks" || entity === "recipes") {
    return entity;
  }

  return undefined;
}

export default useActiveEntityType;
