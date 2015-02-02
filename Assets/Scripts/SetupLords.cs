using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetupLords : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	#endregion components

	public Dictionary<int,Lord> availableLords = new Dictionary<int,Lord>();
	public Dictionary<int,string> lordNames = new Dictionary<int,string>();

	public void setupLord
		(
			int id
			,string name
			,int classid
			,int level
			,int exp
			,bool isAvailable
			)
	{
		Lord newLord = gameObject.AddComponent<Lord>();
		
		newLord.id = id;
		newLord.lordName = name;
		newLord.classid = classid;
		newLord.level = level;

		newLord.addAttack = 0;
		newLord.attack = _gameData.lordClasses[classid].attack;
		newLord.defense = _gameData.lordClasses[classid].defense;
		newLord.speed = _gameData.lordClasses[classid].speed;
		newLord.attackRange = _gameData.lordClasses[classid].attackRange;
		
		newLord.maxHealth = _gameData.lordClasses[classid].maxHealth;
		newLord.currentHealth = _gameData.lordClasses[classid].maxHealth;
		newLord.sprite = _gameData.lordClasses[classid].sprite;
		
		newLord.exp = exp;
		newLord.isAvailable = isAvailable;
		
		_gameData.lords.Add (id,newLord);		
	}
	
	public void createLords(int numOfLords)
	{
		int classID = 0;
		
		//		for(int i = 0; i <= numOfLords; i++)
		for(int i = 0; i < numOfLords; i++)
		{
			//string lordName = lordNames[Random.Range (0,lordNames.Count)];
			string lordName = lordNames[i];
			
			setupLord
				(
					//id
					i
					//name
					,lordName
					//classid
					//,_classes.charClasses[Random.Range (0,_classes.charClasses.Count)].id
					,classID
					//level
					,1
					//exp
					,0
					//isAvailable
					,true
					);
			
			if(classID+1 < _gameData.lordClasses.Count)
			{
				classID++;
			}
			else
			{
				classID = 0;
			}
		}

		createSpecialLords();
	}

	public void createSpecialLords()
	{
		setupLord
		(
			//id
			1001
			//name
			,"Warren Moon"
			//classid
			//,_classes.charClasses[Random.Range (0,_classes.charClasses.Count)].id
			,3
			//level
			,1
			//exp
			,0
			//isAvailable
			,false
		);
	}
		
	public void generateNames()
	{
		lordNames.Add(0,"Delaine Fortino");
		lordNames.Add(1,"Kaila Mutch");
		lordNames.Add(2,"Marco Milam");
		lordNames.Add(3,"Ashlee Squillace");
		lordNames.Add(4,"Katrice Ferro");
		lordNames.Add(5,"Caprice Whitlow");
		lordNames.Add(6,"Chauncey Brummett");
		lordNames.Add(7,"Edgardo Daniell");
		lordNames.Add(8,"Freida Esquivel");
		lordNames.Add(9,"Nikia Seaberg");
		lordNames.Add(10,"Ardath Risser");
		lordNames.Add(11,"Nick Parmley");
		lordNames.Add(12,"Consuelo Morganti");
		lordNames.Add(13,"Nikita Royce");
		lordNames.Add(14,"Gaylord Breeden");
		lordNames.Add(15,"Miguel Hulsey");
		lordNames.Add(16,"Otilia Cambell");
		lordNames.Add(17,"Thea Halfacre");
		lordNames.Add(18,"Roselyn Witkowski");
		lordNames.Add(19,"Renato Verrett");
		lordNames.Add(20,"Lorelei Stahr");
		lordNames.Add(21,"Precious Feiler");
		lordNames.Add(22,"Ethan Vandermolen");
		lordNames.Add(23,"Christina Hodges");
		lordNames.Add(24,"Rosemary Stumbaugh");
		lordNames.Add(25,"Ronald Ferdinand");
		lordNames.Add(26,"Alene Vanderbilt");
		lordNames.Add(27,"Jerri Gaitan");
		lordNames.Add(28,"Barbra Chatham");
		lordNames.Add(29,"Vonnie Crone");
		lordNames.Add(30,"Bea Kaylor");
		lordNames.Add(31,"Phil Philpott");
		lordNames.Add(32,"Edward Wrona");
		lordNames.Add(33,"Magdalena Kalis");
		lordNames.Add(34,"Easter Schacherer");
		lordNames.Add(35,"Paulette Defenbaugh");
		lordNames.Add(36,"Sadye Funari");
		lordNames.Add(37,"Newton Dicarlo");
		lordNames.Add(38,"Johanna Huntington");
		lordNames.Add(39,"Marina Bassler");
		lordNames.Add(40,"Tempie Holoman");
		lordNames.Add(41,"Georgetta Schwartz");
		lordNames.Add(42,"Carolann Dennie");
		lordNames.Add(43,"Drucilla Magdaleno");
		lordNames.Add(44,"Edmund Maresca");
		lordNames.Add(45,"Ellen Courts");
		lordNames.Add(46,"Kandace Currin");
		lordNames.Add(47,"Theresa Baughn");
		lordNames.Add(48,"Claude Fluellen");
		lordNames.Add(49,"Else Harryman");
		lordNames.Add(50,"Chassidy Luoma");
		lordNames.Add(51,"Chana Mercado");
		lordNames.Add(52,"Rosanne Maupin");
		lordNames.Add(53,"Dyan Enciso");
		lordNames.Add(54,"Qiana Odea");
		lordNames.Add(55,"Renetta Basco");
		lordNames.Add(56,"Carmelia Cogburn");
		lordNames.Add(57,"Mathilda Meinhardt");
		lordNames.Add(58,"Ezra Aguero");
		lordNames.Add(59,"Ardella Clouse");
		lordNames.Add(60,"Michelina Stonge");
		lordNames.Add(61,"Kiyoko Schempp");
		lordNames.Add(62,"Wynona Fannon");
		lordNames.Add(63,"Lita Orso");
		lordNames.Add(64,"Marjorie Levitan");
		lordNames.Add(65,"Claudio Glandon");
		lordNames.Add(66,"Jung Umstead");
		lordNames.Add(67,"Jestine Sipp");
		lordNames.Add(68,"Kandace Louis");
		lordNames.Add(69,"Antonio Manchester");
		lordNames.Add(70,"Marta Hennessee");
		lordNames.Add(71,"Harriet Rappaport");
		lordNames.Add(72,"Prudence Howarth");
		lordNames.Add(73,"Piper Goodlow");
		lordNames.Add(74,"Bronwyn Speights");
		lordNames.Add(75,"Gay Shryock");
		lordNames.Add(76,"Janina Philippe");
		lordNames.Add(77,"Deadra Pennebaker");
		lordNames.Add(78,"Liliana Gravitt");
		lordNames.Add(79,"Vernie Hockman");
		lordNames.Add(80,"Delphia Watters");
		lordNames.Add(81,"Gricelda Vazguez");
		lordNames.Add(82,"Dion Pesqueira");
		lordNames.Add(83,"Macie Reineke");
		lordNames.Add(84,"Renato Puleo");
		lordNames.Add(85,"Thad Marietta");
		lordNames.Add(86,"Lavette Taitt");
		lordNames.Add(87,"Kermit Bertin");
		lordNames.Add(88,"Tawna Goodnough");
		lordNames.Add(89,"Rosamaria Yerian");
		lordNames.Add(90,"Homer Kroenke");
		lordNames.Add(91,"Sterling Kappler");
		lordNames.Add(92,"Emory Scheid");
		lordNames.Add(93,"Vergie Leibowitz");
		lordNames.Add(94,"Glenn Shope");
		lordNames.Add(95,"Rufina Arbour");
		lordNames.Add(96,"Pennie Islam");
		lordNames.Add(97,"Mazie Pickford");
		lordNames.Add(98,"Sheron Reves");
		lordNames.Add(99,"Fiona Halley");
		lordNames.Add(100,"Quinn Montague");
		lordNames.Add(101,"Mistie Curtsinger");
		lordNames.Add(102,"Carlene Daughtridge");
		lordNames.Add(103,"Jung Mcfarlain");
		lordNames.Add(104,"Kary Peterkin");
		lordNames.Add(105,"Edith Richburg");
		lordNames.Add(106,"Donnetta Joshua");
		lordNames.Add(107,"Tawanna Situ");
		lordNames.Add(108,"Devora Creel");
		lordNames.Add(109,"Teodoro Fenton");
		lordNames.Add(110,"Jude Nesmith");
		lordNames.Add(111,"Rolanda Jorge");
		lordNames.Add(112,"Roger Nassar");
		lordNames.Add(113,"Ivory Fullam");
		lordNames.Add(114,"Michaela Vail");
		lordNames.Add(115,"Katharina Camberos");
		lordNames.Add(116,"Kellie Prevo");
		lordNames.Add(117,"Claretta Lamberti");
		lordNames.Add(118,"Flavia Carrasquillo");
		lordNames.Add(119,"Marlon Vancuren");
		lordNames.Add(120,"Amina Eilers");
		lordNames.Add(121,"Veola Marra");
		lordNames.Add(122,"Lilla Kantz");
		lordNames.Add(123,"Alecia Deveaux");
		lordNames.Add(124,"Vickey Bryer");
		lordNames.Add(125,"Lavada Brandy");
		lordNames.Add(126,"Allene Althouse");
		lordNames.Add(127,"Alyse Krogman");
		lordNames.Add(128,"Osvaldo Oquin");
		lordNames.Add(129,"Melvin Daves");
		lordNames.Add(130,"Courtney Swanger");
		lordNames.Add(131,"Cherry Hamer");
		lordNames.Add(132,"Bailey Longino");
		lordNames.Add(133,"Shirlene Stoneham");
		lordNames.Add(134,"Illa Deanda");
		lordNames.Add(135,"Patria Washburn");
		lordNames.Add(136,"Allan Leitner");
		lordNames.Add(137,"Norine Khalaf");
		lordNames.Add(138,"Luciano Rickards");
		lordNames.Add(139,"Shanti Weyandt");
		lordNames.Add(140,"Hubert Schlachter");
		lordNames.Add(141,"Despina Steller");
		lordNames.Add(142,"Sherilyn Mirando");
		lordNames.Add(143,"Sierra Cahall");
		lordNames.Add(144,"Dora Michael");
		lordNames.Add(145,"Cristin Pan");
		lordNames.Add(146,"Birgit Felter");
		lordNames.Add(147,"Apryl Ryba");
		lordNames.Add(148,"Dorotha Rosado");
		lordNames.Add(149,"Maisha Penrose");
	}

}
