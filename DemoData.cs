/******************************************************************************
 * Generic Data Structures
 *****************************************************************************/

public class Node<T>
{
	public T data;
	public Node<T>? next;

	public Node(T data) {
		this.data = data;
		next = null;
	}
}

public class RingBuffer<T>
{
	private Node<T>? head;
	private Node<T>? tail;
	private Node<T>? cur;
	private int size;

	public RingBuffer()
	{
		head = cur = null;
		size = 0;
	}

	// get number of elements
	public int getSize()
	{
		return size;
	}


	// add a new element
	public void add(T data)
	{
		if (head == null || tail == null || cur == null)
		{
			head = tail = cur = new Node<T>(data);
		} else {
			tail.next = new Node<T>(data);
			tail = tail.next;
		}

		size++;
	}


	// get the current element
	public T getCur()
	{
		return cur.data;
	}


	// switch to the next element (round-robin)
	public void next()
	{
		if (cur == null)
			return;

		cur = cur.next;
		if (cur == null)
			cur = head;
	}
}



/******************************************************************************
 * Demo specific Datastructures and Data
 *****************************************************************************/

public enum TimeOfDay { Day, Night };
public enum Weather { Sun, Rain };

public class ArousalValence
{
	public int a; /* -10 .. -10 */
	public int v; /* -10 .. +10 */

	public ArousalValence(int a, int v)
	{
		this.a = a;
		this.v = v;
	}

	public override string ToString()
	{
		return "(arousal=" + a + ", valence=" + v + ")";
	}
}


public class DemoDataElem
{
	public int id;
	public DayOfWeek dayOfWeek;
	public TimeOfDay timeOfDay;
	public Weather weather;
	public ArousalValence news;
	public ArousalValence social;
	public ArousalValence dm; /* direct message */
	public int soundLevel; /* simplified to percent */
	public int heartbeat; /* bpm (0 .. no signal) */

	public DemoDataElem(
			int id,
			DayOfWeek dayOfWeek, TimeOfDay timeOfDay, Weather weather,
			ArousalValence news, ArousalValence social, ArousalValence dm,
			int soundLevel, int heartbeat)
	{
		this.id = id;
		this.dayOfWeek = dayOfWeek;
		this.timeOfDay = timeOfDay;
		this.weather = weather;
		this.news = news;
		this.social = social;
		this.dm = dm;
		this.soundLevel = soundLevel;
		this.heartbeat = heartbeat;
	}


	// some preprocessing experiments

	// preprocessing 1 (Carson)
	// Context(Weather/Daytime/News) and Direct(Social/DMs/Sound/Heartbeat)
	public ArousalValence getContext()
	{
		ArousalValence av = new ArousalValence(0,0);

		if (weather == Weather.Sun)
			av.v+=5;
		else if (weather == Weather.Rain)
			av.v-=5;

		if (timeOfDay == TimeOfDay.Day)
			av.a+=5;
		else if (timeOfDay == TimeOfDay.Night)
			av.a-=5;

		av.a = (av.a + news.a)/2;
		av.v = (av.v + news.v)/2;

		return av;
	}

	public ArousalValence getDirect()
	{
		ArousalValence av = new ArousalValence(0,0);
		av.a = (av.a + social.a + dm.a + (soundLevel/10) + ((heartbeat-80)/10))/5;
		av.v = (av.v + social.v + dm.v)/3;
		return av;
	}




	public override string ToString()
	{
		return "(" +
			"id=" + id +
			" dayOfWeek=" + dayOfWeek +
			" timeOfDay=" + timeOfDay +
			" weather=" + weather +
			" news=" + news +
			" social=" + social +
			" directmessage=" + dm +
			" soundLevel=" + soundLevel +
			" hearbeat=" + heartbeat +
			" Carson(context=" + getContext() + ", direct=" + getDirect() + ")" +
			")";
	}

}

public class DemoData : RingBuffer<DemoDataElem>
{
	// data factory

	public static DemoData create()
	{
		int id = 0;
		DemoData dd = new DemoData();

		/*                      id      Day of Week           Time of Day      Weather             News                        Social                   Direct Message           soundlevel   heartbeat    */
		dd.add(new DemoDataElem(id++, DayOfWeek.Monday,      TimeOfDay.Day,   Weather.Rain,    new ArousalValence(-1, -5), new ArousalValence( 3,-3), new ArousalValence(-3,-4),     80,          80      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Monday,      TimeOfDay.Day,   Weather.Sun,     new ArousalValence(-1, -5), new ArousalValence( 5,-1), new ArousalValence(-3,-4),     70,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Monday,      TimeOfDay.Night, Weather.Sun,     new ArousalValence( 1, -7), new ArousalValence( 5,-1), new ArousalValence( 0, 0),     10,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Tuesday,     TimeOfDay.Day,   Weather.Sun,     new ArousalValence( 1, -7), new ArousalValence( 4,-2), new ArousalValence( 0, 0),     70,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Tuesday,     TimeOfDay.Day,   Weather.Sun,     new ArousalValence( 3, -3), new ArousalValence( 4,-2), new ArousalValence( 3,-1),     50,          60      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Tuesday,     TimeOfDay.Night, Weather.Rain,    new ArousalValence( 3, -3), new ArousalValence( 4, 2), new ArousalValence( 3,-1),     10,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Wednesday,   TimeOfDay.Day,   Weather.Rain,    new ArousalValence( 1, -2), new ArousalValence( 4, 2), new ArousalValence( 0, 0),     60,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Wednesday,   TimeOfDay.Day,   Weather.Rain,    new ArousalValence( 1, -2), new ArousalValence(-1,-6), new ArousalValence( 0, 0),     60,          90      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Wednesday,   TimeOfDay.Night, Weather.Rain,    new ArousalValence( 5, -8), new ArousalValence(-1,-6), new ArousalValence( 7,-3),     20,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Thursday,    TimeOfDay.Day,   Weather.Sun,     new ArousalValence( 5, -8), new ArousalValence(-3, 0), new ArousalValence( 7,-3),     70,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Thursday,    TimeOfDay.Day,   Weather.Sun,     new ArousalValence( 5,  6), new ArousalValence(-3, 0), new ArousalValence( 7, 8),     50,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Thursday,    TimeOfDay.Night, Weather.Sun,     new ArousalValence( 5,  6), new ArousalValence( 6, 7), new ArousalValence( 7, 8),     20,         120      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Friday,      TimeOfDay.Day,   Weather.Sun,     new ArousalValence(-3,  1), new ArousalValence( 6, 7), new ArousalValence( 0, 0),     80,         140      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Friday,      TimeOfDay.Day,   Weather.Sun,     new ArousalValence(-3,  1), new ArousalValence( 6, 1), new ArousalValence( 0, 0),     20,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Friday,      TimeOfDay.Night, Weather.Rain,    new ArousalValence(-1, -3), new ArousalValence( 6, 1), new ArousalValence( 1, 1),     30,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Saturday,    TimeOfDay.Day,   Weather.Rain,    new ArousalValence(-1, -3), new ArousalValence( 2, 4), new ArousalValence( 1, 1),     40,          80      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Saturday,    TimeOfDay.Day,   Weather.Sun,     new ArousalValence( 1, -8), new ArousalValence( 2, 4), new ArousalValence( 2,-5),     50,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Saturday,    TimeOfDay.Night, Weather.Sun,     new ArousalValence( 1, -8), new ArousalValence( 1, 7), new ArousalValence( 2,-5),     50,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Sunday,      TimeOfDay.Day,   Weather.Sun,     new ArousalValence( 1, -1), new ArousalValence( 1, 7), new ArousalValence( 0, 0),     10,           0      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Sunday,      TimeOfDay.Day,   Weather.Rain,    new ArousalValence( 1, -1), new ArousalValence( 5, 2), new ArousalValence( 0, 7),     20,          90      ));
		dd.add(new DemoDataElem(id++, DayOfWeek.Sunday,      TimeOfDay.Night, Weather.Rain,    new ArousalValence(-2,  1), new ArousalValence( 5, 2), new ArousalValence( 0, 7),     10,           0      ));

		return dd;
	}

	// small test routine
	public static void test()
	{
		DemoData dd = DemoData.create();

		Console.WriteLine("size {0:D}", dd.getSize());

		for (int i=0; i<dd.getSize()*2; i++) {
			for (int j=0; j<2; j++) {
				DemoDataElem d = dd.getCur();
				Console.WriteLine("access#{0:D}: {1}", j, d);
			}
			Console.WriteLine("select next");
			dd.next();
		}
	}
}
