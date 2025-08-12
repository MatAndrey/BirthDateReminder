export const declension = (value: number, words: string[]) => {
    value = Math.abs(value) % 100;
    var num = value % 10;
    if (value > 10 && value < 20) return words[2];
    if (num > 1 && num < 5) return words[1];
    if (num == 1) return words[0];
    return words[2];
}

export const reminderToString = (reminderNum: number, reminderUnit: number) => {
    switch (reminderUnit) {
        case 0: return declension(reminderNum, ["День", "Дня", "Дней"]);
        case 1: return declension(reminderNum, ["Неделя", "Недели", "Недель"]);
        case 2: return declension(reminderNum, ["Месяц", "Месяца", "Месяцев"]);
        default: return ""
    }
}