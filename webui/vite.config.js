import { defineConfig } from 'vite'
import { fileURLToPath, URL } from 'node:url'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    host: '0.0.0.0',
    port: 8080,
    proxy: {
      '/api': {
        target: "http://127.0.0.1:20000",
        changeOrigin: true,
        ws:true
      }
    }
  },
  build: {
    outDir: '../WebAppStarter/wwwroot',
    emptyOutDir: true
  }
})
