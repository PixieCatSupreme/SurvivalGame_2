[Database: name="BodyParts", id=2]
{
	[Torso:100]
	{
		[Items]
		{
			[LeftLung: 11]
			[RightLung: 11]
			[LeftKidney: 12]
			[RightKidney: 12]
			[Heart: 13]
			[Liver: 14]
			[Flesh: 15]
			[Skin: 16]
		}
	}

	[Head:101]
	{
		[Items]
		{
			[Brain: 7]
			[LeftEye: 8]
			[RightEye: 8]
			[LeftEar: 9]
			[RightEar: 9]
			[Mouth: 10]
			[Skull: 17]
			[HeadSkin:18]
		}
	}
	[Arm:102]
	{
		[Items]
		{
			[Humerus: 4]
			[Radius: 5]
			[Ulna: 6]
			[ArmFlesh: 19]
			[ArmSkin: 20]
		}
	}

	[Leg: 103]
	{

		[Items]
		{
			[Tibia: 0]
			[Fibula: 1]
			[Femur: 2]
			[LegFlesh:21]
			[LegSkin:22]
		}
	}

	[Tibia: 0]
	{
		[Tags]
		{
			[Moving]
			{
				[id=3]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=133]
		} 
	}

	[Fibula: 1]
	{
		[Tags]
		{
			[Moving]
			{
				[id=3]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=40]
		} 
	}

	[Femur: 2]
	{
		[Tags]
		{
			[Moving]
			{
				[id=3]
				[value=1]
			}
		}
		[Material]
		{ 
			[db=1]
			[id=0]
			[Volume=172]		
		}
	}

	[Humerus: 4]
	{
		[Tags]
		{
			[Manipulation]
			{
				[id=4]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=562]
		}
	}

	[Radius: 5]
	{

		[Tags]
		{
			[Manipulation]
			{
				[id=4]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[ id=0]
			[Volume=141]
		}
	}

	[Ulna: 6]
	{
		[Tags]
		{
			[Manipulation]
			{
				[id=4]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=141]
		}
	}
	[Brain: 7]
	{
		[Tags]
		{
			[Consciousness]
			{
				[id=0]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=1130]
		}
	}

	[Eye: 8]
	{
		[Tags]
		{
			[Sight]
			{
				[id=1]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=6]
		}
	}

	[Ear: 9]
	{
		[Tags]
		{
			[Hearing]
			{
				[id=2]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=20]
		}
	}

	[Mouth: 10]
	{
		[Tags]
		{
			[Communication]
			{
				[id=5]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=120]
		}
	}

	[Lung: 11]
	{
		[Tags]
		{
			[Breathing]
			{
				[id=6]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=613]
		}
	}

	[Kidney: 12]
	{
		[Tags]
		{
			[BloodFiltration]
			{
				[id=7]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=132]
		}
	}

	[Heart: 13]
	{
		[Tags]
		{
			[BloodPumping]
			{ 
				[id=8]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=283]
		}
	}

	[Liver: 14]
	{
		[Tags]
		{
			[Metabolism]
			{
				[id=9]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=1462]
		}
	}

	[TorsoFlesh:15]
	{
		[Material]
		{
			[db=1]
			[id=1]
			[volume=1711]
		}
	}

	[TorsoSkin:16]
	{
		[Material]
		{
			[db=1]
			[Id=2]
			[volume=400]
		}
	}

	[Skull: 17]
	{
		[Material]
		{
			[db=1]
			[Id=0]
			[volume=2220]
		}
	}

	[HeadSkin:18]
	{
		[Material]
		{
			[db=1]
			[Id=2]
			[volume=75]
		}
	}

	[ArmFlesh:19]
	{
		[Tags]
		{
			[Manipulation]
			{
				[id=4]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[Id=1]
			[volume=1950]
		}
	}

	[ArmSkin:20]
	{
		[Material]
		{
			[db=1]
			[Id=2]
			[volume=152]
		}
	}

	[LegFlesh:21]
	{
		[Tags]
		{
			[Moving]
			{
				[id=3]
				[value=1]
			}
		}
		[Material]
		{
			[db=1]
			[Id=1]
			[volume=6776]
		}
	}

	[LegSkin:22]
	{
		[Material]
		{
			[db=1]
			[Id=2]
			[volume=552]
		}
	}	
}