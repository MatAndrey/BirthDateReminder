import { useCallback, useEffect, useState } from "react";
import { Outlet } from "react-router";
import { Link } from "react-router";
import AuthService from "./AuthService";
import "./BirthdaysPage.css"
import BirthdayCardList, { type BirthdayItem } from "./BirthdayCardList";

function sortBirthdaysByClosest(birthdays: BirthdayItem[]): BirthdayItem[] {
    const today = new Date();

    return birthdays.sort((a, b) => {
        const dateA = getNextBirthday(a.birthDate, today);
        const dateB = getNextBirthday(b.birthDate, today);

        return dateA.getTime() - dateB.getTime();
    });
}

function getNextBirthday(birthDateStr: string, today: Date): Date {
    const [year, month, day] = birthDateStr.split('-').map(Number);
    const birthDate = new Date(today.getFullYear(), month - 1, day);

    if (birthDate < today) {
        birthDate.setFullYear(today.getFullYear() + 1);
    }

    return birthDate;
}

function BirthdaysPage() {
    const [todayBDs, setTodayBDs] = useState<BirthdayItem[]>([]);
    const [futureBDs, setFutureBDs] = useState<BirthdayItem[]>([]);
    const [isLoading, setIsLoading] = useState(false);

    const loadBirthdays = useCallback(async () => {
        setIsLoading(true)
        AuthService.fetchWithAuth("/api/birthday")
            .then(resp => resp.json())
            .then((data: BirthdayItem[]) => {
                setTodayBDs([])
                setFutureBDs([])
                const now = new Date()
                sortBirthdaysByClosest(data)
                    .forEach(el => {
                        const date = new Date(el.birthDate);
                        if (date.getMonth() === now.getMonth() &&
                            date.getDate() == now.getDate()) {
                            setTodayBDs(prev => prev ? [...prev, el] : [el])
                        } else {
                            setFutureBDs(prev => prev ? [...prev, el] : [el])
                        }
                    })
                setIsLoading(false)
            });
    }, [])

    useEffect(() => {
        loadBirthdays()
    }, [loadBirthdays]);

    
    return (
        <>
            <div className="options">
                <Link to="/birthdays/create" className="primary button">Добавить</Link>
            </div>

            {isLoading ? <p>Загрузка...</p> :
                <>
                    <h2>Сегодняшние дни рождения</h2>
                    {
                        todayBDs.length === 0 ?
                            <p>Сегодня нет дней рождения</p> :
                            <BirthdayCardList birthdays={todayBDs} />
                    }
                    <h2>Предстоящие дни рождения</h2>
                    {
                        futureBDs.length === 0 ?
                            <p>Нет предстоящих дней рождения</p> :
                            <BirthdayCardList birthdays={futureBDs} />
                    }
                </>                    
            }
            <Outlet context={ loadBirthdays } />
        </>)
}

export default BirthdaysPage;