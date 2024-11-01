import { wrapGrid } from 'animate-css-grid'
import { DotNetObjectReference } from './DotNetObjectReference.ts'
import { PhazorFunctions } from './PhazorFunctions.ts'

// @ts-ignore
var Phazor: PhazorFunctions = window.Phazor ??= {} as PhazorFunctions

Phazor.subscribeOnScrollFinished = function (elementId, handler: DotNetObjectReference): void {
    const el = document.getElementById(elementId);

    el.addEventListener('scroll', async () => {
        if (el.scrollHeight - el.offsetHeight - el.scrollTop < 1) {
            await handler.invokeMethodAsync('OnScrollFinished');
        }
    });
}

Phazor.isContentFillingParent = function (elementId: string): boolean {
    const el = document.getElementById(elementId);
    return el.offsetHeight < el.scrollHeight;
}

Phazor.wrapGrid = function (elementId: string): void {
    const el = document.getElementById(elementId)
    wrapGrid(el)
}