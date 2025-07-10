import { Link } from "react-router";

export interface BirthdayItem {
    id: number;
    name: string;
    birthDate: string;
    imagePath?: string;
    ownerId: number
}

export default function BirthdayCardList({ birthdays }: { birthdays: BirthdayItem[] }) {
    return birthdays.map(birthday => (
        <Link className="birthday-card" to={`/birthdays/${birthday.id}`} key={birthday.id}>
            <img src={birthday.imagePath || './DefaultImage.webp'} alt={`${birthday.name}'s photo`} />
            <div className="info">
                <p className="name">{birthday.name}</p>
                <p className="date">{new Date(birthday.birthDate).toLocaleDateString()}</p>
            </div>
        </Link>))
}