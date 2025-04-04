/**
 * Utility functions to handle conversion between PascalCase (C# API) and camelCase (JS/TS conventions)
 */

type CamelToSnakeCase<S extends string> = 
  S extends `${infer T}${infer U}` ? 
    T extends Capitalize<T> ?
      `${Lowercase<T>}${U}` : 
      S : 
    S;

/**
 * Converts a property from PascalCase to camelCase
 * Example: "PropertyName" -> "propertyName"
 */
export function toCamelCase(str: string): string {
  return str.charAt(0).toLowerCase() + str.slice(1);
}

/**
 * Maps an object with PascalCase keys to an object with camelCase keys
 * Example: { PropertyName: 'value' } -> { propertyName: 'value' }
 */
export function toCamelCaseKeys<T extends Record<string, any>>(obj: T): { [K in keyof T as CamelToSnakeCase<string & K>]: T[K] } {
  if (!obj || typeof obj !== 'object' || obj === null) {
    return obj;
  }

  if (Array.isArray(obj)) {
    return obj.map(toCamelCaseKeys) as any;
  }

  return Object.entries(obj).reduce((acc, [key, value]) => {
    const camelKey = toCamelCase(key);
    
    // Recursively handle nested objects and arrays
    const camelValue = value && typeof value === 'object' ? toCamelCaseKeys(value) : value;
    
    return {
      ...acc,
      [camelKey]: camelValue
    };
  }, {}) as any;
}

/**
 * Type utility to convert an interface from PascalCase to camelCase properties
 * Use this for API response models
 */
export type PascalToCamelCase<T> = {
  [K in keyof T as CamelToSnakeCase<string & K>]: T[K] extends Record<string, any>
    ? PascalToCamelCase<T[K]>
    : T[K] extends Array<infer U>
      ? U extends Record<string, any>
        ? Array<PascalToCamelCase<U>>
        : T[K]
      : T[K]
};