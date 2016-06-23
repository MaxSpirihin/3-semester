import java.awt.Graphics;

//прямоугольники, здесь основа программы
public class Rectangles {

	private Rectangle[] rectangles;



	public Rectangles(int count) {
		rectangles = new Rectangle[count];

		for (int i = 0; i < count; i++) {
			this.rectangles[i] = new Rectangle(0, 0);
		}
	}



	public Rectangle get(int number) {
		return rectangles[number];
	}



	public int Size() {
		return rectangles.length;
	}



	// находит сумму площадей всех прямоугольников, извлекает корень и если он
	// не целый, то возвращает false
	private boolean SquareIs() {
		int square = 0;
		for (int i = 0; i < rectangles.length; i++) {
			square += rectangles[i].getWidth() * rectangles[i].getHeight();
		}

		if (Math.round(Math.sqrt(square)) == Math.sqrt(square))
			return true;
		return false;
	}



	// возвращает сторону квадрата, который должен будет получиться
	private int GetSizeOfSquare() {
		if (!this.SquareIs())
			return 0;
		int square = 0;
		for (int i = 0; i < rectangles.length; i++) {
			square += rectangles[i].getWidth() * rectangles[i].getHeight();
		}
		return (int) (Math.round(Math.sqrt(square)));
	}



	// рисует все прямоугольники
	public void DrawAll(Graphics g) {
		for (int i = 0; i < Size(); i++) {
			// if (get(i).IsInSquare())
			get(i).Draw(g);
		}
	}



	// ДЛЯ ТЕСТА!!! рисует точку на экране
	public void DrawPoint(Graphics g, int x, int y) {
		g.drawOval((x + Main.START_X) * Main.PIXELS_IN_COORDINATE
				- Main.PIXELS_IN_COORDINATE / 2, (y + Main.START_Y - 1 / 2)
				* Main.PIXELS_IN_COORDINATE - Main.PIXELS_IN_COORDINATE / 2,
				Main.PIXELS_IN_COORDINATE, Main.PIXELS_IN_COORDINATE);
	}



	// находит левый верхний угол для вставки следующего прямоугольника
	public Coordinates GetTopLeftAngle() {

		int X = GetSizeOfSquare();
		int Y = GetSizeOfSquare();
		int currentX = 0;
		int currentY = 0;

		// проходимся по каждому прямоугольнику
		for (int numOfRect = 0; numOfRect < Size(); numOfRect++) {

			// работать с прямоугольником будем только, если он вставлен
			if (get(numOfRect).IsInSquare()) {

				// сначала проверяем нижний левый угол
				currentX = get(numOfRect).getX();
				currentY = get(numOfRect).getY() + get(numOfRect).getHeight();

				// проверим, не поставлен ли в эту точку другой прямоугольник
				// уже
				boolean placeIsEmpty = true;
				for (int i = 0; i < Size(); i++) {
					if ((get(i).IsInSquare()) && (get(i).getX() == currentX)
							&& (get(i).getY() == currentY)) {
						placeIsEmpty = false;
					}
				}

				// проверим не попала ли точка на конец квадрат
				if ((currentX == this.GetSizeOfSquare())
						|| (currentY == this.GetSizeOfSquare()))
					placeIsEmpty = false;

				// еще одно условие, не могу нормально объяснить
				for (int i = 0; i < Size(); i++) {
					if ((get(i).getY() + get(i).getHeight() == currentY)
							&& (get(i).getX() + get(i).getWidth() == currentX)
							&& (get(i).IsInSquare()))
						placeIsEmpty = false;
				}

				// если угол пуст, проверим, не является ли он искомым на данном
				// этапе
				if (placeIsEmpty) {
					if (currentX < X) {
						X = currentX;
						Y = currentY;
					} else if ((currentX == X) && (currentY < Y)) {
						X = currentX;
						Y = currentY;
					}// end else if
				}// end place if

				// проделаем ту же хрень для верхнего правого угла
				currentX = get(numOfRect).getX() + get(numOfRect).getWidth();
				currentY = get(numOfRect).getY();

				// проверим, не поставлен ли в эту точку другой прямоугольник
				// уже
				placeIsEmpty = true;
				for (int i = 0; i < Size(); i++) {
					if ((get(i).IsInSquare()) && (get(i).getX() == currentX)
							&& (get(i).getY() == currentY)) {
						placeIsEmpty = false;
					}
				}

				// проверим не попала ли точка на конец квадрат
				if ((currentX == this.GetSizeOfSquare())
						|| (currentX == this.GetSizeOfSquare()))
					placeIsEmpty = false;

				// еще одно условие, не могу нормально объяснить
				for (int i = 0; i < Size(); i++) {
					if ((get(i).getX() + get(i).getWidth() == currentX)
							&& (get(i).getY() + get(i).getHeight() == currentY)
							&& (get(i).IsInSquare()))
						placeIsEmpty = false;
				}

				// если угол пуст, проверим, не является ли он искомым на данном
				// этапе
				if (placeIsEmpty) {
					if (currentX < X) {
						X = currentX;
						Y = currentY;
					} else if ((currentX == X) && (currentY < Y)) {
						X = currentX;
						Y = currentY;
					}// end else if
				}// end place if

			}// end main if

		}// end for

		// если прямоугольников пока нет в квадрате
		if ((currentX == 0) && (currentY == 0)) {
			X = 0;
			Y = 0;
		}

		return new Coordinates(X, Y);
	}



	// проверяет не пересекаются ли прямоугольники
	private boolean RectanglesAreBlocked(int first, int second) {
		if ((first >= Size()) || (second >= Size()))
			return false;

		// если левая сторона первого правее правой второго или наоборот, они не
		// могут пересекаться
		if ((get(first).getX() >= get(second).getX() + get(second).getWidth())
				|| (get(second).getX() >= get(first).getX()
						+ get(first).getWidth()))
			return false;

		// аналогично для высоты
		if ((get(first).getY() >= get(second).getY() + get(second).getHeight())
				|| (get(second).getY() >= get(first).getY()
						+ get(first).getHeight()))
			return false;

		// ни одно условие не выполнено, они пересекаются
		return true;
	}



	private boolean ThisRectangleBlockedByAnyOtherRectangleOrOutsideSquare(
			int number) {
		for (int i = 0; i < Size(); i++) {

			if ((get(i).IsInSquare()) && (RectanglesAreBlocked(i, number))
					&& (i != number))
				return true;

		}

		// итак, прямоугольник не пересекает другие, проверим, что он внутри
		// квадрата.
		if ((get(number).getX() < 0)
				|| (get(number).getX() + get(number).getWidth() > this
						.GetSizeOfSquare())
				|| (get(number).getY() < 0)
				|| (get(number).getY() + get(number).getHeight() > this
						.GetSizeOfSquare()))
			return true;

		return false;
	}



	// строит квадрат и возвращает false если не удалось
	public boolean BuildSquare() {
		RecursionComplete = false;
		if (!this.SquareIs())
			return false;

		// вставляем первый прямоугольник, далее пусть работает рекурсия
		int i = 0;
		while ((!RecursionComplete)&&(i<rectangles.length)) {
			PutTheRectangle(i);
			if (!RecursionComplete)
				get(i).DeleteFromSquare();
			i++;
		}

		return RecursionComplete;

	}

	// переменная отсечения рекурсии, проще сделать ее классовой (приватная),
	// чем гонять ее по методам
	private boolean RecursionComplete = false;



	// главная процедура вставки прямоугольника
	public void PutTheRectangle(int pos) {

		if (!RecursionComplete) {
			// вставляем
			get(pos).SetCoordinates(this.GetTopLeftAngle().x,
					this.GetTopLeftAngle().y);
			get(pos).PutToSquare();

			if (this.ThisRectangleBlockedByAnyOtherRectangleOrOutsideSquare(pos)) {
				// прерываем рекурсию, блоки перекрываются, перестановка не
				// подходит
			}

			else {
				// вставка успешна, идем дальше

				// проверим, не построек ли квадрат
				boolean allRectanglesInSquare = true;

				// вставляем каждый из невставленных
				for (int i = 0; i < Size(); i++) {
					if (!get(i).IsInSquare()) {
						// невставленные есть
						allRectanglesInSquare = false;
						PutTheRectangle(i);
						// если все не кончилось, удаляем
						if (!RecursionComplete)
							get(i).DeleteFromSquare();
					}
				}

				if (allRectanglesInSquare) {
					// квадрат построен
					RecursionComplete = true;
				}
			}
		}
	}

}
