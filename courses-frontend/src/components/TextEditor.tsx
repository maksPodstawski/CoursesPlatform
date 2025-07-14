import React, { useRef, useEffect } from 'react';
import '../styles/TextEditor.css';

interface TextEditorProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
}

const TextEditor: React.FC<TextEditorProps> = ({ value, onChange, placeholder }) => {
  const editorRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (editorRef.current && editorRef.current.innerHTML !== value) {
      editorRef.current.innerHTML = value || '';
    }
  }, [value]);

  const handleCommand = (command: string, arg?: string) => {
    if (editorRef.current) {
      editorRef.current.focus();
      document.execCommand(command, false, arg);
      onChange(editorRef.current.innerHTML);
    }
  };

  const handleInput = () => {
    if (editorRef.current) {
      onChange(editorRef.current.innerHTML);
    }
  };

  return (
    <div className="text-editor-root">
      <div className="text-editor-toolbar">
        <button type="button" title="Bold" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('bold')}><b>B</b></button>
        <button type="button" title="Italic" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('italic')}><i>I</i></button>
        <button type="button" title="Underline" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('underline')}><u>U</u></button>
        <button type="button" title="Heading" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('formatBlock', 'H2')}>H2</button>
        <button type="button" title="Normal text" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('formatBlock', 'P')}>Normal</button>
        <button type="button" title="Align left" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('justifyLeft')}>⯇</button>
        <button type="button" title="Align center" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('justifyCenter')}>≡</button>
        <button type="button" title="Align right" onMouseDown={e => e.preventDefault()} onClick={() => handleCommand('justifyRight')}>⯈</button>
      </div>
      <div
        className="text-editor-content"
        contentEditable
        ref={editorRef}
        onInput={handleInput}
        onBlur={handleInput}
        data-placeholder={placeholder}
        style={{
          minHeight: 120,
          border: '1px solid #333',
          padding: 8,
          borderRadius: 4,
          background: '#181818',
          color: '#e0e0e0',
          direction: 'ltr',
          textAlign: 'left',
        }}
        spellCheck={true}
        suppressContentEditableWarning
      />
    </div>
  );
};

export default TextEditor; 