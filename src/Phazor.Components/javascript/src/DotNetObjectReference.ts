export interface DotNetObjectReference {
    invokeMethodAsync(methodName: string): Promise<any>
}