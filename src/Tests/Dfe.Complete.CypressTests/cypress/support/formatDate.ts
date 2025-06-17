export function toDisplayDate(date: Date): string {
    return date.toLocaleDateString("en-gb", { day: "numeric", month: "long", year: "numeric" });
}

export function significateDateToDisplayDate(significantDate: string): string {
    const date = new Date(significantDate);
    return toDisplayDate(date);
}
