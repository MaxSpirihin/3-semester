import java.awt.Graphics;

//��������������, ����� ������ ���������
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



	// ������� ����� �������� ���� ���������������, ��������� ������ � ���� ��
	// �� �����, �� ���������� false
	private boolean SquareIs() {
		int square = 0;
		for (int i = 0; i < rectangles.length; i++) {
			square += rectangles[i].getWidth() * rectangles[i].getHeight();
		}

		if (Math.round(Math.sqrt(square)) == Math.sqrt(square))
			return true;
		return false;
	}



	// ���������� ������� ��������, ������� ������ ����� ����������
	private int GetSizeOfSquare() {
		if (!this.SquareIs())
			return 0;
		int square = 0;
		for (int i = 0; i < rectangles.length; i++) {
			square += rectangles[i].getWidth() * rectangles[i].getHeight();
		}
		return (int) (Math.round(Math.sqrt(square)));
	}



	// ������ ��� ��������������
	public void DrawAll(Graphics g) {
		for (int i = 0; i < Size(); i++) {
			// if (get(i).IsInSquare())
			get(i).Draw(g);
		}
	}



	// ��� �����!!! ������ ����� �� ������
	public void DrawPoint(Graphics g, int x, int y) {
		g.drawOval((x + Main.START_X) * Main.PIXELS_IN_COORDINATE
				- Main.PIXELS_IN_COORDINATE / 2, (y + Main.START_Y - 1 / 2)
				* Main.PIXELS_IN_COORDINATE - Main.PIXELS_IN_COORDINATE / 2,
				Main.PIXELS_IN_COORDINATE, Main.PIXELS_IN_COORDINATE);
	}



	// ������� ����� ������� ���� ��� ������� ���������� ��������������
	public Coordinates GetTopLeftAngle() {

		int X = GetSizeOfSquare();
		int Y = GetSizeOfSquare();
		int currentX = 0;
		int currentY = 0;

		// ���������� �� ������� ��������������
		for (int numOfRect = 0; numOfRect < Size(); numOfRect++) {

			// �������� � ��������������� ����� ������, ���� �� ��������
			if (get(numOfRect).IsInSquare()) {

				// ������� ��������� ������ ����� ����
				currentX = get(numOfRect).getX();
				currentY = get(numOfRect).getY() + get(numOfRect).getHeight();

				// ��������, �� ��������� �� � ��� ����� ������ �������������
				// ���
				boolean placeIsEmpty = true;
				for (int i = 0; i < Size(); i++) {
					if ((get(i).IsInSquare()) && (get(i).getX() == currentX)
							&& (get(i).getY() == currentY)) {
						placeIsEmpty = false;
					}
				}

				// �������� �� ������ �� ����� �� ����� �������
				if ((currentX == this.GetSizeOfSquare())
						|| (currentY == this.GetSizeOfSquare()))
					placeIsEmpty = false;

				// ��� ���� �������, �� ���� ��������� ���������
				for (int i = 0; i < Size(); i++) {
					if ((get(i).getY() + get(i).getHeight() == currentY)
							&& (get(i).getX() + get(i).getWidth() == currentX)
							&& (get(i).IsInSquare()))
						placeIsEmpty = false;
				}

				// ���� ���� ����, ��������, �� �������� �� �� ������� �� ������
				// �����
				if (placeIsEmpty) {
					if (currentX < X) {
						X = currentX;
						Y = currentY;
					} else if ((currentX == X) && (currentY < Y)) {
						X = currentX;
						Y = currentY;
					}// end else if
				}// end place if

				// ��������� �� �� ����� ��� �������� ������� ����
				currentX = get(numOfRect).getX() + get(numOfRect).getWidth();
				currentY = get(numOfRect).getY();

				// ��������, �� ��������� �� � ��� ����� ������ �������������
				// ���
				placeIsEmpty = true;
				for (int i = 0; i < Size(); i++) {
					if ((get(i).IsInSquare()) && (get(i).getX() == currentX)
							&& (get(i).getY() == currentY)) {
						placeIsEmpty = false;
					}
				}

				// �������� �� ������ �� ����� �� ����� �������
				if ((currentX == this.GetSizeOfSquare())
						|| (currentX == this.GetSizeOfSquare()))
					placeIsEmpty = false;

				// ��� ���� �������, �� ���� ��������� ���������
				for (int i = 0; i < Size(); i++) {
					if ((get(i).getX() + get(i).getWidth() == currentX)
							&& (get(i).getY() + get(i).getHeight() == currentY)
							&& (get(i).IsInSquare()))
						placeIsEmpty = false;
				}

				// ���� ���� ����, ��������, �� �������� �� �� ������� �� ������
				// �����
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

		// ���� ��������������� ���� ��� � ��������
		if ((currentX == 0) && (currentY == 0)) {
			X = 0;
			Y = 0;
		}

		return new Coordinates(X, Y);
	}



	// ��������� �� ������������ �� ��������������
	private boolean RectanglesAreBlocked(int first, int second) {
		if ((first >= Size()) || (second >= Size()))
			return false;

		// ���� ����� ������� ������� ������ ������ ������� ��� ��������, ��� ��
		// ����� ������������
		if ((get(first).getX() >= get(second).getX() + get(second).getWidth())
				|| (get(second).getX() >= get(first).getX()
						+ get(first).getWidth()))
			return false;

		// ���������� ��� ������
		if ((get(first).getY() >= get(second).getY() + get(second).getHeight())
				|| (get(second).getY() >= get(first).getY()
						+ get(first).getHeight()))
			return false;

		// �� ���� ������� �� ���������, ��� ������������
		return true;
	}



	private boolean ThisRectangleBlockedByAnyOtherRectangleOrOutsideSquare(
			int number) {
		for (int i = 0; i < Size(); i++) {

			if ((get(i).IsInSquare()) && (RectanglesAreBlocked(i, number))
					&& (i != number))
				return true;

		}

		// ����, ������������� �� ���������� ������, ��������, ��� �� ������
		// ��������.
		if ((get(number).getX() < 0)
				|| (get(number).getX() + get(number).getWidth() > this
						.GetSizeOfSquare())
				|| (get(number).getY() < 0)
				|| (get(number).getY() + get(number).getHeight() > this
						.GetSizeOfSquare()))
			return true;

		return false;
	}



	// ������ ������� � ���������� false ���� �� �������
	public boolean BuildSquare() {
		RecursionComplete = false;
		if (!this.SquareIs())
			return false;

		// ��������� ������ �������������, ����� ����� �������� ��������
		int i = 0;
		while ((!RecursionComplete)&&(i<rectangles.length)) {
			PutTheRectangle(i);
			if (!RecursionComplete)
				get(i).DeleteFromSquare();
			i++;
		}

		return RecursionComplete;

	}

	// ���������� ��������� ��������, ����� ������� �� ��������� (���������),
	// ��� ������ �� �� �������
	private boolean RecursionComplete = false;



	// ������� ��������� ������� ��������������
	public void PutTheRectangle(int pos) {

		if (!RecursionComplete) {
			// ���������
			get(pos).SetCoordinates(this.GetTopLeftAngle().x,
					this.GetTopLeftAngle().y);
			get(pos).PutToSquare();

			if (this.ThisRectangleBlockedByAnyOtherRectangleOrOutsideSquare(pos)) {
				// ��������� ��������, ����� �������������, ������������ ��
				// ��������
			}

			else {
				// ������� �������, ���� ������

				// ��������, �� �������� �� �������
				boolean allRectanglesInSquare = true;

				// ��������� ������ �� �������������
				for (int i = 0; i < Size(); i++) {
					if (!get(i).IsInSquare()) {
						// ������������� ����
						allRectanglesInSquare = false;
						PutTheRectangle(i);
						// ���� ��� �� ���������, �������
						if (!RecursionComplete)
							get(i).DeleteFromSquare();
					}
				}

				if (allRectanglesInSquare) {
					// ������� ��������
					RecursionComplete = true;
				}
			}
		}
	}

}
