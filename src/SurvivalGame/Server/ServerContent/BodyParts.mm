[Database: name="BodyParts", id=3]
{
	[Tibia: 0]
	{
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=133]
		} 
	}

	[Fibula: 1]
	{
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=40]
		} 
	}

	[Femur: 2]
	{
		[Material]
		{ 
			[db=1]
			[id=0]
			[Volume=172]		
		}
	}

	[Humerus: 4]
	{
		[Material]
		{
			[db=1]
			[id=0]
			[Volume=562]
		}
	}

	[Radius: 5]
	{
		[Material]
		{
			[db=1]
			[ id=0]
			[Volume=141]
		}
	}

	[Ulna: 6]
	{
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
			[Consciousness: id=0, value=1]
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
			[Sight: id=1, value=1]
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
			[Hearing: id=2, value=1]
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
			[Communication: id=5, value=1]
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
			[Breathing: id=6, value=1]
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
			[BloodFiltration: id=7, value=1]
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
			[BloodPumping: id=8, value=1]
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
			[Metabolism: id=9, value=1]
		}
		[Material]
		{
			[db=1]
			[id=1]
			[Volume=1462]
		}
	}

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
			[Biomass: 1]
			{
				[Flesh]
				{
					[Id=1]
					[volume=1711]
				}
				[Skin]
				{
					[Id=2]
					[volume=400]
				}
			}
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
			[Biomass: 1]
			{
				[Bone]
				{
					[id=0]
					[volume=2220]
				}
				[Skin]
				{
					[id=2]
					[volume=75]
				}
			}
		}
	}
	[Arm:102]
	{
		[Tags]
		{
			[Manipulation: id=4, value 1]
		}

		[Items]
		{
			[Humerus: 4]
			[Radius: 5]
			[Ulna: 6]

			[Biomass: 1]
			{
				[Flesh]
				{
					[id=1]
					[volume=1950]
				}
				[Skin]
				{
					[id=2]
					[volume=152]
				}
			}
		}
	}

	[Leg: 103]
	{
		[Tags]
		{
			[Moving: id=3, value=1]
		}

		[Items]
		{
			[Tibia: 0]
			[Fibula: 1]
			[Femur: 2]

			[Biomass: 1]
			{
				[Flesh]
				{
					[id=1]
					[volume=6776]
				}
				[Skin]
				{
					[id=2]
					[volume=552]
				}
			}
		}
	}	
}