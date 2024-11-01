import { resolve } from 'path'
import { defineConfig } from 'vite'

export default defineConfig({
    define: {
        'process.env.NODE_ENV': '"production"'
    },
    build: {
        minify: 'esbuild',
        lib: {
            entry: resolve(__dirname, 'src/Phazor.Components.ts'),
            name: 'Phazor.Components',
            fileName: 'Phazor.Components',
        },
        rollupOptions: {
            output: {
                manualChunks: undefined,
                inlineDynamicImports: true,
                entryFileNames: '[name].js',
                assetFileNames: '[name].[ext]',
            },
        }
    },
})