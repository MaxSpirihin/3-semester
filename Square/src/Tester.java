//класс для тестирования
public class Tester {

	//возвращает прямоугольники с тестовыми размерами
	public static Rectangles GetTestInstanceOfRectangles(int testNumber) {
		Rectangles rectangles = null;

		switch (testNumber) {
		case 1:
			rectangles = new Rectangles(5);
			rectangles.get(4).SetSize(10, 30);
			rectangles.get(3).SetSize(30, 10);
			rectangles.get(2).SetSize(30, 10);
			rectangles.get(0).SetSize(20, 20);
			rectangles.get(1).SetSize(10, 30);
			break;
		case 2:
			rectangles = new Rectangles(9);
			rectangles.get(0).SetSize(20, 30);
			rectangles.get(1).SetSize(60, 30);
			rectangles.get(2).SetSize(10, 10);
			rectangles.get(3).SetSize(10, 10);
			rectangles.get(4).SetSize(10, 20);
			rectangles.get(5).SetSize(30, 10);
			rectangles.get(6).SetSize(10, 20);
			rectangles.get(7).SetSize(10, 10);
			rectangles.get(8).SetSize(10, 20);
			break;
		case 3:
			rectangles = new Rectangles(11);
			rectangles.get(0).SetSize(10, 50);
			rectangles.get(1).SetSize(10, 10);
			rectangles.get(2).SetSize(20, 10);
			rectangles.get(3).SetSize(20, 10);
			rectangles.get(4).SetSize(20, 20);
			rectangles.get(5).SetSize(20, 50);
			rectangles.get(6).SetSize(40, 40);
			rectangles.get(7).SetSize(20, 20);
			rectangles.get(8).SetSize(20, 10);
			rectangles.get(9).SetSize(10, 10);
			rectangles.get(10).SetSize(20, 10);
			break;
		case 4:
			rectangles = new Rectangles(12);
			rectangles.get(0).SetSize(60, 40);
			rectangles.get(1).SetSize(20, 60);
			rectangles.get(2).SetSize(20, 20);
			rectangles.get(3).SetSize(10, 10);
			rectangles.get(4).SetSize(10, 10);
			rectangles.get(5).SetSize(10, 10);
			rectangles.get(6).SetSize(20, 10);
			rectangles.get(7).SetSize(20, 30);
			rectangles.get(8).SetSize(10, 10);
			rectangles.get(9).SetSize(40, 10);
			rectangles.get(10).SetSize(20, 20);
			rectangles.get(11).SetSize(20, 20);
			break;
		}

		return rectangles;

	}
}
