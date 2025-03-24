/**
 * Formats duration in milliseconds to a readable format
 * @param ms Duration in milliseconds
 * @returns Formatted string representation (e.g., "123ms", "1.2s", "2m30s")
 */
export const formatDuration = (ms: number): string => {
    if (ms < 1000) {
        return `${ms}ms`;
    } else if (ms < 60000) {
        return `${(ms / 1000).toFixed(1)}s`;
    } else {
        const minutes = Math.floor(ms / 60000);
        const seconds = ((ms % 60000) / 1000).toFixed(0);
        return `${minutes}m${seconds}s`;
    }
};

/**
 * Formats a date object to a readable time string
 * @param date Date object to format
 * @returns Formatted time string (e.g., "2:30 PM")
 */
export const formatTime = (date: Date): string => {
    return date.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
};

/**
 * Formats a number with commas as thousands separators
 * @param num Number to format
 * @returns Formatted number string with thousands separators
 */
export const formatNumber = (num: number): string => {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
};