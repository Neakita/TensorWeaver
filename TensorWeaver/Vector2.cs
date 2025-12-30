using System.Numerics;

namespace TensorWeaver;

public readonly struct Vector2<T> : IEquatable<Vector2<T>> where T : INumber<T>, IConvertible
{
	public static explicit operator Vector2<float>(Vector2<T> value)
	{
		return new Vector2<float>(value.X.ToSingle(null), value.Y.ToSingle(null));
	}

	public static explicit operator Vector2<int>(Vector2<T> value)
	{
		return new Vector2<int>(value.X.ToInt32(null), value.Y.ToInt32(null));
	}

	public static bool operator ==(Vector2<T> left, Vector2<T> right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Vector2<T> left, Vector2<T> right)
	{
		return !left.Equals(right);
	}

	public static Vector2<T> operator +(Vector2<T> first, Vector2<T> second)
	{
		return new Vector2<T>(first.X + second.X, first.Y + second.Y);
	}

	public static Vector2<T> operator -(Vector2<T> first, Vector2<T> second)
	{
		return new Vector2<T>(first.X - second.X, first.Y - second.Y);
	}

	public static Vector2<T> operator +(Vector2<T> first, T second)
	{
		return new Vector2<T>(first.X + second, first.Y + second);
	}

	public static Vector2<T> operator -(Vector2<T> first, T second)
	{
		return new Vector2<T>(first.X - second, first.Y - second);
	}

	public static Vector2<T> operator *(Vector2<T> first, T second)
	{
		return new Vector2<T>(first.X * second, first.Y * second);
	}

	public static Vector2<T> operator /(Vector2<T> first, T second)
	{
		return new Vector2<T>(first.X / second, first.Y / second);
	}

	public static Vector2<T> operator *(Vector2<T> first, Vector2<T> second)
	{
		return new Vector2<T>(first.X * second.X, first.Y * second.Y);
	}

	public static Vector2<T> operator /(Vector2<T> first, Vector2<T> second)
	{
		return new Vector2<T>(first.X / second.X, first.Y / second.Y);
	}

	public T X { get; }
	public T Y { get; }

	public Vector2(T x, T y)
	{
		X = x;
		Y = y;
	}

	public Vector2<T> WithX(T value)
	{
		return new Vector2<T>(value, Y);
	}

	public Vector2<T> WithY(T value)
	{
		return new Vector2<T>(X, value);
	}

	public Vector2<T> Clamp(Vector2<T> min, Vector2<T> max)
	{
		return new Vector2<T>(T.Clamp(X, min.X, max.X), T.Clamp(Y, min.Y, max.Y));
	}

	public Vector2<float> ToSingle(IFormatProvider? provider = null)
	{
		return new Vector2<float>(X.ToSingle(provider), Y.ToSingle(provider));
	}

	public bool Equals(Vector2<T> other)
	{
		return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y);
	}

	public override bool Equals(object? obj)
	{
		return obj is Vector2<T> other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}