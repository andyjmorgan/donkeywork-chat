// A simple script to build only the AgentBuilder and related files
const fs = require('fs');
const path = require('path');

// Files we want to copy
const filesToCopy = [
  'src/services/api/agentService.ts',
  'src/services/api/index.ts',
  'src/components/agent/AgentBuilder.tsx',
];

// Create a dist directory
const distDir = path.join(__dirname, 'dist');
if (!fs.existsSync(distDir)) {
  fs.mkdirSync(distDir);
}

// Copy each file
filesToCopy.forEach(file => {
  const sourcePath = path.join(__dirname, file);
  const targetPath = path.join(distDir, path.basename(file));
  
  try {
    const content = fs.readFileSync(sourcePath, 'utf8');
    fs.writeFileSync(targetPath, content, 'utf8');
    console.log(`Copied ${file} to ${targetPath}`);
  } catch (error) {
    console.error(`Error copying ${file}:`, error);
  }
});

console.log('Build completed successfully!');