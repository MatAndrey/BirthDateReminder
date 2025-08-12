import { Link } from "react-router";
import { declension } from "./utils";

export interface BirthdayItem {
    id: number;
    name: string;
    birthDate: string;
    imagePath?: string;
    ownerId: number
}

export default function BirthdayCardList({ birthdays }: { birthdays: BirthdayItem[] }) {
    return birthdays.map(birthday => {
        const bdDate = new Date(birthday.birthDate)
        const dateNow = new Date()
        const years = dateNow.getFullYear() - bdDate.getFullYear()
        return (
            <Link className="birthday-card" to={`/birthdays/${birthday.id}`} key={birthday.id}>
                <img src={birthday.imagePath || './DefaultImage.webp'} alt={`${birthday.name}'s photo`} />
                <div className="info">
                    <p className="name">{birthday.name} - {years} {declension(years, ["Год", "Года", "Лет"])}</p>
                    <p className="date">{bdDate.toLocaleDateString()}</p>
                </div>
            </Link>)
    })
}