export function toDisplayDate(date: Date): string {
    return date.toLocaleDateString("en-gb", { day: "numeric", month: "long", year: "numeric" });
}

export function significateDateToDisplayDate(significantDate: string): string {
    const date = new Date(significantDate);
    return toDisplayDate(date);
}

export function getDateMonthsFromNow(months: number): Date {
    const baseDate = new Date();
    const currentYear = baseDate.getFullYear();
    const currentMonth = baseDate.getMonth();

    // Calculate target year and month properly
    const totalMonths = currentMonth + months;
    const targetYear = currentYear + Math.floor(totalMonths / 12);
    const targetMonth = totalMonths % 12;

    // Create date using UTC to avoid timezone issues
    return new Date(Date.UTC(targetYear, targetMonth, 1));
}

export function getSignificantDateString(months: number): string {
    return getDateMonthsFromNow(months).toISOString().split("T")[0]; // YYYY-MM-DD format
}

export function getDisplayDateString(months: number): string {
    const date = getDateMonthsFromNow(months);
    return toDisplayDate(date);
}

export function getMonthNumber(months: number): number {
    return getDateMonthsFromNow(months).getMonth() + 1; // Convert to 1-indexed
}

export function getYearNumber(months: number): number {
    return getDateMonthsFromNow(months).getFullYear();
}
