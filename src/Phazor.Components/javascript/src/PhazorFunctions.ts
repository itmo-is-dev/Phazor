import { DotNetObjectReference } from './DotNetObjectReference.ts'

export interface PhazorFunctions{
    subscribeOnScrollFinished(elementId: string, handler: DotNetObjectReference): void

    isContentFillingParent(elementId: string): boolean

    wrapGrid(elementId: string): void
}