/// <summary>
/// Documentation (revision 1) - Last Update: 1.4.2021 11:36
/// This interface acts as an abstraction for the classes <see cref="Line"/> and <see cref="LineSegment"/>.
/// Usually you will use the former concrete types, because it is much better to know the object on both sides of
/// the intersection method. A <see cref="Line"/> intersecting a <see cref="Line"/> is very different from a <see cref="LineSegment"/> intersecting
/// a <see cref="LineSegment"/>.
/// 
/// By knowing with what concrete class you are dealing with, you are aware of the logic associated
/// with the line-to-line intersection. For example if you try to get the intersection for two <see cref="Line"/>s, you can be certain that you won't
/// get an intersection ONLY if the lines are parallel (+ the other special cases you know of).
/// 
/// Nevertheless if you want an abstraction and do not care about the left hand operand of the intersection, by which
/// I mean the reference, since as of now the parameter for the intersection methods is fully concrete, you can use this interface. But
/// you won't be able to tell that the lines do not intersect because the intersection is not within the interval of a LineSegment or because
/// they are parallel.
/// 
/// </summary>
public interface ICanIntersectLineAndSegment : ICanIntersectLine, ICanIntersectLineSegment
{

}