using tl121pet.Entities.Models;

public class PersonComparer : EqualityComparer<Person>
{
    public override bool Equals(Person p1, Person p2)
    {
        if (p1 == p2)
            return true;

        if ((p1 == null && p2 != null) || (p1 != null && p2 == null))
            return false;

        return p1.PersonId == p2.PersonId;
    }

    public override int GetHashCode(Person p1) => p1 != null && p1.PersonId != null ? p1.PersonId.GetHashCode() : 0;
}