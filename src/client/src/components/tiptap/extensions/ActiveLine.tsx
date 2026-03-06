import { Extension } from "@tiptap/core";
import { Plugin, PluginKey } from "prosemirror-state";
import { Decoration, DecorationSet } from "prosemirror-view";

export const ActiveLine = Extension.create({
  name: "activeLine",

  addProseMirrorPlugins() {
    return [
      new Plugin({
        key: new PluginKey("activeLine"),

        props: {
          decorations(state) {
            const { $from } = state.selection;
            const node = $from.node($from.depth);

            const decoration = Decoration.node(
              $from.before($from.depth),
              $from.after($from.depth),
              { class: "active-line" },
            );

            return DecorationSet.create(state.doc, [decoration]);
          },
        },
      }),
    ];
  },
});
