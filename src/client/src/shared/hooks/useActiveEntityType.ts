import { useLocation } from "react-router-dom";
import { EntityType } from "../types/entityType";

function useActiveEntityType(): EntityType {
  const { pathname } = useLocation(); // "/notes /tasks ..."
  const entity = pathname.split("/")[1]; // notes

  if (entity === "notes" || entity === "recipes" || entity === "tasks") {
    return entity;
  }

  return null;
}

export default useActiveEntityType;
