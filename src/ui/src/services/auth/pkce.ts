/**
 * Generates a random code verifier string for PKCE.
 * @returns A random code verifier string.
 */
export const generateCodeVerifier = (): string => {
  const array = new Uint8Array(32);
  window.crypto.getRandomValues(array);
  return base64UrlEncode(array);
};

/**
 * Generates a code challenge from a code verifier using the S256 method.
 * @param codeVerifier The code verifier to generate the challenge from.
 * @returns A promise that resolves to the code challenge.
 */
export const generateCodeChallenge = async (codeVerifier: string): Promise<string> => {
  // Hash the code verifier with SHA-256
  const encoder = new TextEncoder();
  const data = encoder.encode(codeVerifier);
  const digest = await window.crypto.subtle.digest('SHA-256', data);
  
  // Base64-URL encode the hash
  return base64UrlEncode(new Uint8Array(digest));
};

/**
 * Encodes a Uint8Array to a base64url string.
 * @param buffer The buffer to encode.
 * @returns The base64url encoded string.
 */
const base64UrlEncode = (buffer: Uint8Array): string => {
  // First convert the buffer to a regular base64 string
  const base64 = window.btoa(String.fromCharCode(...new Uint8Array(buffer)));
  
  // Then convert to base64url format
  return base64
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '');
};