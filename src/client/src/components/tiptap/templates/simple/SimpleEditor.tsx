"use client";

import { useEffect, useRef, useState } from "react";
import {
  EditorContent,
  EditorContext,
  JSONContent,
  useEditor,
} from "@tiptap/react";

// --- Tiptap Core Extensions ---
import { StarterKit } from "@tiptap/starter-kit";
import { Image } from "@tiptap/extension-image";
import { TaskItem, TaskList } from "@tiptap/extension-list";
import { TextAlign } from "@tiptap/extension-text-align";
import { Typography } from "@tiptap/extension-typography";
import { Highlight } from "@tiptap/extension-highlight";
import { Subscript } from "@tiptap/extension-subscript";
import { Superscript } from "@tiptap/extension-superscript";
import { Selection } from "@tiptap/extensions";

// --- UI Primitives ---
import { Button } from "@/components/tiptap/ui-primitive/button";
import { Spacer } from "@/components/tiptap/ui-primitive/spacer";
import {
  Toolbar,
  ToolbarGroup,
  ToolbarSeparator,
} from "@/components/tiptap/ui-primitive/toolbar";

// --- Tiptap Node ---
import { ImageUploadNode } from "@/components/tiptap/nodes/image-upload-node/image-upload-node-extension";
import { HorizontalRule } from "@/components/tiptap/nodes/horizontal-rule-node/horizontal-rule-node-extension";
import "@/components/tiptap/nodes/blockquote-node/blockquote-node.scss";
import "@/components/tiptap/nodes/code-block-node/code-block-node.scss";
import "@/components/tiptap/nodes/horizontal-rule-node/horizontal-rule-node.scss";
import "@/components/tiptap/nodes/list-node/list-node.scss";
import "@/components/tiptap/nodes/image-node/image-node.scss";
import "@/components/tiptap/nodes/heading-node/heading-node.scss";
import "@/components/tiptap/nodes/paragraph-node/paragraph-node.scss";

// --- Tiptap UI ---
import { HeadingDropdownMenu } from "@/components/tiptap/ui/heading-dropdown-menu";
import { ImageUploadButton } from "@/components/tiptap/ui/image-upload-button";
import { ListDropdownMenu } from "@/components/tiptap/ui/list-dropdown-menu";
import { BlockquoteButton } from "@/components/tiptap/ui/blockquote-button";
import { CodeBlockButton } from "@/components/tiptap/ui/code-block-button";
import {
  ColorHighlightPopover,
  ColorHighlightPopoverContent,
  ColorHighlightPopoverButton,
} from "@/components/tiptap/ui/color-highlight-popover";
import {
  LinkPopover,
  LinkContent,
  LinkButton,
} from "@/components/tiptap/ui/link-popover";
import { MarkButton } from "@/components/tiptap/ui/mark-button";
import { TextAlignButton } from "@/components/tiptap/ui/text-align-button";
import { UndoRedoButton } from "@/components/tiptap/ui/undo-redo-button";

// --- Icons ---
import { ArrowLeftIcon } from "@/components/tiptap/icons/ArrowLeftIcon";
import { HighlighterIcon } from "@/components/tiptap/icons/HighlighterIcon";
import { LinkIcon } from "@/components/tiptap/icons/LinkIcon";

// --- Hooks ---
import { useIsBreakpoint } from "@/components/tiptap/hooks/useIsBreakpoint";
import { useWindowSize } from "@/components/tiptap/hooks/useWindowSize";
import { useCursorVisibility } from "@/components/tiptap/hooks/useCursorVisibility";

// --- Components ---
import { ThemeToggle } from "@/components/tiptap/templates/simple/ThemeToggle";

// --- Lib ---
import {
  handleImageUpload,
  MAX_FILE_SIZE,
} from "@/components/tiptap/lib/tiptap-utils";

// --- Styles ---
import "@/components/tiptap/templates/simple/simple-editor.scss";

import content from "@/components/tiptap/templates/simple/data/content.json";
import EditorHeader from "../../ui/editor-header/EditorHeader";
import { ActiveLine } from "../../extensions/ActiveLine";

const MainToolbarContent = ({
  onHighlighterClick,
  onLinkClick,
  isMobile,
}: {
  onHighlighterClick: () => void;
  onLinkClick: () => void;
  isMobile: boolean;
}) => {
  return (
    <>
      <Spacer />

      <ToolbarGroup>
        <UndoRedoButton action="undo" />
        <UndoRedoButton action="redo" />
      </ToolbarGroup>

      <ToolbarSeparator />

      <ToolbarGroup>
        <HeadingDropdownMenu levels={[1, 2, 3, 4]} portal={isMobile} />
        <ListDropdownMenu
          types={["bulletList", "orderedList", "taskList"]}
          portal={isMobile}
        />
        <BlockquoteButton />
        <CodeBlockButton />
      </ToolbarGroup>

      <ToolbarSeparator />

      <ToolbarGroup>
        <MarkButton type="bold" showShortcut={true} />
        <MarkButton type="italic" showShortcut={true} />
        <MarkButton type="strike" showShortcut={true} />
        <MarkButton type="code" showShortcut={true} />
        <MarkButton type="underline" showShortcut={true} />
        {!isMobile ? (
          <ColorHighlightPopover />
        ) : (
          <ColorHighlightPopoverButton onClick={onHighlighterClick} />
        )}
        {!isMobile ? <LinkPopover /> : <LinkButton onClick={onLinkClick} />}
      </ToolbarGroup>

      <ToolbarSeparator />

      <ToolbarGroup>
        <MarkButton type="superscript" showShortcut={true} />
        <MarkButton type="subscript" showShortcut={true} />
      </ToolbarGroup>

      <ToolbarSeparator />

      <ToolbarGroup>
        <TextAlignButton align="left" showShortcut={true} />
        <TextAlignButton align="center" showShortcut={true} />
        <TextAlignButton align="right" showShortcut={true} />
        <TextAlignButton align="justify" showShortcut={true} />
      </ToolbarGroup>

      <ToolbarSeparator />

      <ToolbarGroup>
        <ImageUploadButton text="Add" showShortcut={true} />
      </ToolbarGroup>

      <Spacer />

      {isMobile && <ToolbarSeparator />}

      <ToolbarGroup>
        <ThemeToggle />
      </ToolbarGroup>
    </>
  );
};

const MobileToolbarContent = ({
  type,
  onBack,
}: {
  type: "highlighter" | "link";
  onBack: () => void;
}) => (
  <>
    <ToolbarGroup>
      <Button variant="ghost" onClick={onBack}>
        <ArrowLeftIcon className="tiptap-button-icon" />
        {type === "highlighter" ? (
          <HighlighterIcon className="tiptap-button-icon" />
        ) : (
          <LinkIcon className="tiptap-button-icon" />
        )}
      </Button>
    </ToolbarGroup>

    <ToolbarSeparator />

    {type === "highlighter" ? (
      <ColorHighlightPopoverContent />
    ) : (
      <LinkContent />
    )}
  </>
);

interface SimpleEditorProps {
  initialContent?: JSONContent | null;
  noteTitle?: string;
  onChange?: (content: JSONContent) => void;
}

export function SimpleEditor({
  initialContent,
  noteTitle,
  onChange,
}: SimpleEditorProps) {
  const isMobile = useIsBreakpoint();
  const { height } = useWindowSize();
  const [mobileView, setMobileView] = useState<"main" | "highlighter" | "link">(
    "main",
  );
  const toolbarRef = useRef<HTMLDivElement>(null);

  const editor = useEditor({
    immediatelyRender: false,
    editorProps: {
      attributes: {
        autocomplete: "off",
        autocorrect: "off",
        autocapitalize: "off",
        "aria-label": "Main content area, start typing to enter text.",
        class:
          "simple-editor flex-1 p-12 pb-[30vh] max-w-full font-[DM_Sans,sans-serif] max-sm:p-4 max-sm:px-6 max-sm:pb-[30vh]",
      },
    },
    extensions: [
      StarterKit.configure({
        horizontalRule: false,
        link: {
          openOnClick: false,
          enableClickSelection: true,
        },
      }),
      HorizontalRule,
      TextAlign.configure({ types: ["heading", "paragraph"] }),
      TaskList,
      TaskItem.configure({ nested: true }),
      Highlight.configure({ multicolor: true }),
      Image,
      Typography,
      Superscript,
      Subscript,
      Selection,
      ImageUploadNode.configure({
        accept: "image/*",
        maxSize: MAX_FILE_SIZE,
        limit: 3,
        upload: handleImageUpload,
        onError: (error) => console.error("Upload failed:", error),
      }),
      ActiveLine, // Подсветка активной строки
    ],
    content: initialContent ?? content,
    onUpdate({ editor }) {
      const json = editor.getJSON();
      onChange?.(json);
    },
  });

  // Обновление состояние редактор (содержимого)
  useEffect(() => {
    if (!editor) return;

    if (initialContent != null) {
      editor.commands.setContent(initialContent);
    } else {
      editor.commands.clearContent();
    }
  }, [editor, initialContent]);

  const rect = useCursorVisibility({
    editor,
    overlayHeight: toolbarRef.current?.getBoundingClientRect().height ?? 0,
  });

  useEffect(() => {
    if (!isMobile && mobileView !== "main") {
      setMobileView("main");
    }
  }, [isMobile, mobileView]);

  return (
    <div
      className="simple-editor-wrapper flex flex-col w-screen h-screen overflow-auto overscroll-none p-0 font-normal not-italic text-(--tt-theme-text) bg-(--tt-bg-color) [font-optical-sizing:auto]"
      style={{ fontFamily: '"Inter", sans-serif' }}
    >
      <EditorContext.Provider value={{ editor }}>
        <div className="sticky top-0 z-10 bg-(--tt-bg-color)">
          <EditorHeader title={noteTitle} />
          <Toolbar
            ref={toolbarRef}
            style={{
              ...(isMobile
                ? {
                    bottom: `calc(100% - ${height - rect.y}px)`,
                  }
                : {}),
            }}
          >
            {mobileView === "main" ? (
              <MainToolbarContent
                onHighlighterClick={() => setMobileView("highlighter")}
                onLinkClick={() => setMobileView("link")}
                isMobile={isMobile}
              />
            ) : (
              <MobileToolbarContent
                type={mobileView === "highlighter" ? "highlighter" : "link"}
                onBack={() => setMobileView("main")}
              />
            )}
          </Toolbar>
        </div>

        {/* Отображение контента редактора */}
        <EditorContent
          editor={editor}
          role="presentation"
          className="simple-editor-content max-w-4xl w-full mx-auto h-full flex flex-col flex-1 bg-(--tt-bg-color)"
        />
      </EditorContext.Provider>
    </div>
  );
}
