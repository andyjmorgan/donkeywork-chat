import { useRef, useEffect, useState, useCallback } from 'react';
import Editor, { OnMount } from '@monaco-editor/react';
import { Button } from 'primereact/button';
import { Dropdown } from 'primereact/dropdown';
import './TemplateEditor.css';

// For TypeScript
declare global {
  interface Window {
    __getTemplateEditorNodes?: () => string[];
  }
}

// Global variable to track if Scriban template language has been registered
// This persists across component re-renders and recreations
// Global tracked disposables to prevent duplicate registration
let globalDisposables: Array<any> = [];

// Function to clean up disposables
function cleanupDisposables() {
  globalDisposables.forEach(disposable => {
    try {
      if (disposable && typeof disposable.dispose === 'function') {
        disposable.dispose();
      }
    } catch (e) {
      console.error("Error disposing provider:", e);
    }
  });
  globalDisposables = [];
}

// Registry to track if completion providers have been registered for this editor instance
// Each editor will have its own unique ID to track registration state
let editorId = 0;
const registeredEditors = new Map<number, boolean>();

// Get a unique ID for this editor instance
function getNextEditorId() {
  return editorId++;
}

interface TemplateEditorProps {
  value: string;
  onChange: (value: string) => void;
  connectedNodes: string[];
}

// Define snippets for reuse
const TEMPLATE_SNIPPETS = [
  {
    name: 'foreach loop',
    value: [
      '{{ for $1 in $2 }}',
      '  $3',
      '{{ end }}'
    ].join('\n'),
    description: 'Scriban foreach loop'
  },
  {
    name: 'if-elseif-else block',
    value: [
      '{{ if $1 }}',
      '  $2',
      '{{ elseif $3 }}',
      '  $4',
      '{{ else }}',
      '  $5',
      '{{ end }}'
    ].join('\n'),
    description: 'Scriban if-elseif-else block'
  },
  {
    name: 'conditional block',
    value: [
      '{{ if $1 }}',
      '  $2',
      '{{ else }}',
      '  $3',
      '{{ end }}'
    ].join('\n'),
    description: 'Simple if-else conditional block'
  },
  {
    name: 'function definition',
    value: [
      '{{ func $1($2) }}',
      '  $3',
      '{{ end }}'
    ].join('\n'),
    description: 'Scriban function definition'
  },
  {
    name: 'include template',
    value: '{{ include "$1" }}',
    description: 'Scriban include template'
  },
  {
    name: 'raw block',
    value: [
      '{{ raw }}',
      '  $1',
      '{{ end }}'
    ].join('\n'),
    description: 'Scriban raw block'
  }
];

const TemplateEditor = ({ value, onChange, connectedNodes }: TemplateEditorProps) => {
  const editorRef = useRef<any>(null);
  const [selectedSnippet, setSelectedSnippet] = useState<any>(null);
  const [currentEditorId] = useState<number>(() => getNextEditorId());
  const disposablesRef = useRef<Array<any>>([]);
  const [isDarkMode, setIsDarkMode] = useState<boolean>(false);
  
  // Function to register this editor instance's completion providers
  const registerCompletionProviders = useCallback((monaco: any) => {
    if (!monaco || !editorRef.current) {
      console.warn("Cannot register providers: Monaco or editor not initialized");
      return;
    }
    
    // If already registered for this instance, skip
    if (registeredEditors.get(currentEditorId)) {
      return;
    }
    
    // Clean up any previous disposables for this editor
    disposablesRef.current.forEach(d => {
      try {
        if (d && typeof d.dispose === 'function') {
          d.dispose();
        }
      } catch (e) {
        console.error("Error disposing provider:", e);
      }
    });
    disposablesRef.current = [];
    
    // Check if 'scriban' language exists
    if (!monaco.languages.getLanguages().some((lang: any) => lang.id === 'scriban')) {
      monaco.languages.register({ id: 'scriban' });
    }
    
    try {
      // Register syntax highlighting
      const tokenProvider = monaco.languages.setMonarchTokensProvider('scriban', {
        tokenizer: {
          root: [
            [/\{\{/, { token: 'delimiter.curly', bracket: '@open', next: '@scribanInner' }],
            [/./, 'text'],
          ],
          scribanInner: [
            [/\}\}/, { token: 'delimiter.curly', bracket: '@close', next: '@pop' }],
            [/[\w]+/, 'variable.scriban'],
            [/\./, 'delimiter.dot'],
            [/ /, 'white'],
          ],
        }
      });
      if (tokenProvider) disposablesRef.current.push(tokenProvider);
      
      // Set language configuration
      const langConfig = monaco.languages.setLanguageConfiguration('scriban', {
        autoClosingPairs: [
          { open: '{', close: '}' },
          { open: '{{', close: '}}' },
          { open: '[', close: ']' },
          { open: '(', close: ')' },
          { open: '"', close: '"' },
          { open: "'", close: "'" }
        ],
        brackets: [
          ['{', '}'],
          ['{{', '}}'],
          ['[', ']'],
          ['(', ')']
        ],
        surroundingPairs: [
          { open: '{', close: '}' },
          { open: '{{', close: '}}' },
          { open: '[', close: ']' },
          { open: '(', close: ')' },
          { open: '"', close: '"' },
          { open: "'", close: "'" }
        ]
      });
      if (langConfig) disposablesRef.current.push(langConfig);
      
      // Create node completion provider
      const nodeProvider = monaco.languages.registerCompletionItemProvider('scriban', {
        triggerCharacters: [' '],
        provideCompletionItems: (model: any, position: any) => {
          const textUntilPosition = model.getValueInRange({
            startLineNumber: position.lineNumber,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          });
          
          // Check if we're inside a handlebars expression after opening {{
          const match = textUntilPosition.match(/\{\{\s*$/);
          if (match) {
            // Create a range for the current word
            const range = {
              startLineNumber: position.lineNumber,
              startColumn: position.column,
              endLineNumber: position.lineNumber,
              endColumn: position.column
            };
            
            // Get connected nodes for suggestions
            const nodeSuggestions = connectedNodes.map(nodeName => ({
              label: nodeName,
              kind: monaco.languages.CompletionItemKind.Variable,
              insertText: nodeName,
              documentation: `Reference to ${nodeName} node`,
              detail: `Node: ${nodeName}`,
              range
            }));
            
            return { suggestions: nodeSuggestions };
          }
          return { suggestions: [] };
        }
      });
      disposablesRef.current.push(nodeProvider);
      
      // Create dot completion provider
      const dotProvider = monaco.languages.registerCompletionItemProvider('scriban', {
        triggerCharacters: ['.'],
        provideCompletionItems: (model: any, position: any) => {
          const textUntilPosition = model.getValueInRange({
            startLineNumber: position.lineNumber,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          });
          
          // Check if we're typing "." after a node name inside handlebars
          const match = textUntilPosition.match(/\{\{\s*([a-zA-Z0-9_-]+)\.$/);
          if (match) {
            const range = {
              startLineNumber: position.lineNumber,
              startColumn: position.column,
              endLineNumber: position.lineNumber,
              endColumn: position.column
            };
            
            return {
              suggestions: [
                {
                  label: 'Text',
                  kind: monaco.languages.CompletionItemKind.Property,
                  insertText: 'Text',
                  documentation: 'The text property of the node',
                  detail: 'Property: Text',
                  range
                }
              ]
            };
          }
          return { suggestions: [] };
        }
      });
      disposablesRef.current.push(dotProvider);
      
      // Register snippets provider
      const snippetsProvider = monaco.languages.registerCompletionItemProvider('scriban', {
        triggerCharacters: ['!'],
        provideCompletionItems: (model: any, position: any) => {
          const word = model.getWordUntilPosition(position);
          const range = {
            startLineNumber: position.lineNumber,
            endLineNumber: position.lineNumber,
            startColumn: word.startColumn,
            endColumn: word.endColumn
          };
          
          // Create snippet suggestions
          return {
            suggestions: TEMPLATE_SNIPPETS.map(snippet => ({
              label: snippet.name,
              kind: monaco.languages.CompletionItemKind.Snippet,
              insertText: snippet.value,
              insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
              documentation: snippet.description,
              range
            }))
          };
        }
      });
      disposablesRef.current.push(snippetsProvider);
      
      // Mark as registered
      registeredEditors.set(currentEditorId, true);
      
    } catch (e) {
      console.error(`Error registering providers for editor ${currentEditorId}:`, e);
    }
  }, [connectedNodes, currentEditorId]);
  
  // Cleanup when component unmounts
  useEffect(() => {
    return () => {
      disposablesRef.current.forEach(d => {
        try {
          if (d && typeof d.dispose === 'function') {
            d.dispose();
          }
        } catch (e) {
          console.error("Error disposing provider:", e);
        }
      });
      disposablesRef.current = [];
      registeredEditors.delete(currentEditorId);
    };
  }, [currentEditorId]);
  
  // Detect dark mode from document and apply theme
  useEffect(() => {
    // Check for dark mode by seeing if document has a class indicating dark mode
    // or checking computed style on body/html element
    const checkDarkMode = () => {
      const isDark = document.documentElement.classList.contains('dark-mode') || 
        document.body.classList.contains('dark-mode') ||
        window.matchMedia('(prefers-color-scheme: dark)').matches ||
        // Check for specific PrimeReact theme class
        document.documentElement.classList.contains('dark') ||
        // Check background color of body
        getComputedStyle(document.body).backgroundColor.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/)?.[1] < '128';
      
      setIsDarkMode(!!isDark);
      
      // Apply theme if editor is available
      // We use type assertion since we know monaco will be available when mounted
      if (editorRef.current && (window as any).monaco) {
        (window as any).monaco.editor.setTheme(isDark ? 'donkeywork-dark' : 'donkeywork-light');
      }
    };
    
    // Initial check
    checkDarkMode();
    
    // Set up a mutation observer to watch for theme changes
    const observer = new MutationObserver(checkDarkMode);
    observer.observe(document.documentElement, { 
      attributes: true, 
      attributeFilter: ['class'] 
    });
    observer.observe(document.body, { 
      attributes: true, 
      attributeFilter: ['class'] 
    });
    
    // Clean up
    return () => observer.disconnect();
  }, []);

  // Function to handle editor mount
  const handleEditorDidMount: OnMount = (editor, monaco) => {
    editorRef.current = editor;
    
    // Define theme for dark mode
    monaco.editor.defineTheme('donkeywork-dark', {
      base: 'vs-dark',
      inherit: true,
      rules: [
        { token: 'delimiter.curly', foreground: '#569cd6' },
        { token: 'variable.scriban', foreground: '#9cdcfe' }
      ],
      colors: {
        'editor.background': '#1e1e1e',
        'editor.foreground': '#d4d4d4',
        'editorCursor.foreground': '#d4d4d4',
        'editor.lineHighlightBackground': '#2a2d2e',
        'editorLineNumber.foreground': '#858585',
        'editor.selectionBackground': '#264f78',
        'editor.inactiveSelectionBackground': '#3a3d41'
      }
    });
    
    // Define theme for light mode
    monaco.editor.defineTheme('donkeywork-light', {
      base: 'vs',
      inherit: true,
      rules: [
        { token: 'delimiter.curly', foreground: '#0000ff' },
        { token: 'variable.scriban', foreground: '#008080' }
      ],
      colors: {
        'editor.background': '#ffffff',
        'editor.foreground': '#000000'
      }
    });
    
    // Check if dark mode is currently active
    const isDark = document.documentElement.classList.contains('dark-mode') || 
      document.body.classList.contains('dark-mode') ||
      window.matchMedia('(prefers-color-scheme: dark)').matches || 
      document.documentElement.classList.contains('dark') ||
      getComputedStyle(document.body).backgroundColor.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/)?.[1] < '128';
    
    // Apply theme based on the current mode  
    monaco.editor.setTheme(isDark ? 'donkeywork-dark' : 'donkeywork-light');
    
    // Wait for a short time to ensure the editor is fully initialized before registering providers
    setTimeout(() => {
      if (editor && monaco) {
        // Register completion providers after the editor is fully mounted
        registerCompletionProviders(monaco);
      }
    }, 100);
  };

  // Function to insert node template at cursor position
  const insertNodeTemplate = (nodeName: string) => {
    if (editorRef.current) {
      const editor = editorRef.current;
      const position = editor.getPosition();
      const text = `{{ ${nodeName} }}`;
      
      editor.executeEdits('insert-text', [
        {
          range: {
            startLineNumber: position.lineNumber,
            startColumn: position.column,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          },
          text
        }
      ]);
      
      // Set cursor position after inserted text
      const newPosition = {
        lineNumber: position.lineNumber,
        column: position.column + text.length
      };
      editor.setPosition(newPosition);
      
      // Focus back on editor
      editor.focus();
    }
  };
  
  // Function to insert a code snippet at cursor position
  const insertSnippet = (snippet: string) => {
    if (editorRef.current && snippet) {
      const editor = editorRef.current;
      const position = editor.getPosition();
      
      editor.executeEdits('insert-snippet', [
        {
          range: {
            startLineNumber: position.lineNumber,
            startColumn: position.column,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          },
          text: snippet
        }
      ]);
      
      // Focus back on editor
      editor.focus();
      
      // Reset the dropdown
      setSelectedSnippet(null);
    }
  };

  // Define Monaco editor options
  const editorOptions = {
    minimap: { enabled: false },
    scrollBeyondLastLine: false,
    fontFamily: 'monospace',
    fontSize: 14,
    lineNumbers: 'on' as const,
    automaticLayout: true,
    formatOnPaste: true,
    wordWrap: 'on' as 'on',
    autoClosingBrackets: 'always' as const,
    autoClosingQuotes: 'always' as const,
    autoClosingPairs: [
      { open: '{', close: '}' },
      { open: '{{', close: '}}' },
      { open: '[', close: ']' },
      { open: '(', close: ')' },
      { open: '"', close: '"' },
      { open: "'", close: "'" }
    ],
    suggest: {
      showWords: false,
      snippetsPreventQuickSuggestions: false,
      showIcons: true,
      filterGraceful: true,
      selectionMode: 'always' as 'always',
      insertMode: 'insert' as 'insert'
    },
    quickSuggestions: {
      other: true,
      comments: true,
      strings: true
    },
    quickSuggestionsDelay: 10,
    snippetSuggestions: 'top' as 'top',
    acceptSuggestionOnEnter: 'on' as 'on',
    tabCompletion: 'on' as 'on',
    wordBasedSuggestions: 'off' as 'off',
    contextmenu: true,
    suggestSelection: 'first' as 'first',
    suggestOnTriggerCharacters: true
  };

  return (
    <div className="template-editor">
      <div className="monaco-editor-container" style={{ border: '1px solid var(--surface-border, #ced4da)', borderRadius: '4px', height: '300px' }}>
        <Editor
          height="300px"
          defaultLanguage="scriban"
          language="scriban"
          value={value}
          onChange={val => onChange(val || '')}
          onMount={handleEditorDidMount}
          options={editorOptions}
          key={`editor-${currentEditorId}`} // Stable key based on editor ID
        />
      </div>
      
      <div style={{ marginTop: '10px', display: 'flex', gap: '8px', flexWrap: 'wrap', alignItems: 'center' }}>
        <div style={{ display: 'flex', gap: '8px', alignItems: 'center', marginRight: '16px' }}>
          <div style={{ fontWeight: 'bold' }}>Insert Snippet: </div>
          <Dropdown
            value={selectedSnippet}
            options={TEMPLATE_SNIPPETS}
            onChange={(e) => {
              setSelectedSnippet(e.value);
              if (e.value) insertSnippet(e.value.value);
            }}
            optionLabel="name"
            placeholder="Select a snippet"
            style={{ minWidth: '200px' }}
          />
        </div>
        
        <div style={{ fontWeight: 'bold' }}>Insert Connected Node: </div>
        {connectedNodes.length > 0 ? (
          connectedNodes.map(nodeName => (
            <Button
              key={nodeName}
              label={nodeName}
              size="small"
              outlined
              onClick={() => insertNodeTemplate(nodeName)}
            />
          ))
        ) : (
          <div style={{ color: 'var(--text-color-secondary, #999)', fontStyle: 'italic' }}>No connected nodes available</div>
        )}
      </div>
    </div>
  );
};

export default TemplateEditor;