import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { resolve } from 'path';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react({
      // Add the react refresh functionality
      jsxRuntime: 'automatic',
      // Remove fastRefresh as it's not in the Options type
    }),
  ],
  resolve: {
    alias: {
      // If you have any specific path aliases in your CRA project
      '@': resolve(__dirname, 'src'),
    },
  },
  server: {
    allowedHosts: ['chat.donkeywork.dev'],
    port: 8001,
    host: true,
    proxy: {
      '/api/': {
        target: 'http://localhost:8881',
        changeOrigin: true,
        secure: false,
        ws: true,
      }
    },
  },
  build: {
    outDir: 'build', // To match CRA's default output directory
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/setupTests.ts'],
    css: true,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'html'],
    },
  },
});