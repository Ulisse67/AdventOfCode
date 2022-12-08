namespace Extensions;

public static class MatrixExtensions {
	public static T[,] Make<T, S>(this IEnumerable<S> lines) where S : IEnumerable<T>
	{
		var (rows, cols) = (lines.Count(), lines.First().Count());

		T[,] matrix = new T[rows, cols];
		int r = 0;
		foreach (var l in lines) {
			int c = 0;
			foreach (var ch in l) matrix[r, c++] = ch;
			r++;
		}

		return matrix;
	}

	public static T[,] Fill<T>(this T[,] ar, T val)
	{
		int d0 = ar.GetLength(0), d1 = ar.GetLength(1);
		for (int r = 0; r < d0; r++) for (int c = 0; c < d1; c++) ar[r, c] = val;

		return ar;
	}

	static T[,] Clone<T>(this T[,] lhs)
	{
		T[,] res = new T[lhs.GetLength(0), lhs.GetLength(1)];
		for (int r = 0; r < lhs.GetLength(0); r++)
			for (int c = 0; c < lhs.GetLength(1); c++) res[r, c] = lhs[r, c];

		return res;
	}

	public static void Print<T>(this T[,] mx)
	{
		int d0 = mx.GetLength(0), d1 = mx.GetLength(1);
		for (int r = 0; r < d0; r++) {
			Console.Write($"{r,5}: ");
			for (int c = 0; c < d1; c++) {
				Console.Write(mx[r, c]);
			}

			Console.WriteLine();
		}
	}
}
