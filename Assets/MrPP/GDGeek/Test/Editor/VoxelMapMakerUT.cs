﻿using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GDGeek;
using System.IO;

public class VoxelMapMakerUT {

	[Test] 
	public void SplitVoxelTest(){

		Vector3Int a = new Vector3Int (3, 4, 5);
		Vector3Int b = new Vector3Int (1, 2, 3);
		Assert.AreEqual (a - b, new Vector3Int (2, 2, 2));

		FileStream sr2 = new FileStream (".//Assets//Voxel//grass.bytes", FileMode.OpenOrCreate, FileAccess.Read);
		System.IO.BinaryReader br2 = new System.IO.BinaryReader (sr2); 
		VoxelStruct vs = MagicaVoxelFormater.ReadFromBinary (br2).structure;


		SplitVoxel split = new SplitVoxel (vs);

		split.addBox(new Vector3Int(0,0,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(0,17,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(0,34,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(0,54,0), new Vector3Int(16,16,3));


		split.addBox(new Vector3Int(20,0,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(20,17,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(20,34,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(20,54,0), new Vector3Int(16,16,3));


		split.addBox(new Vector3Int(37,0,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(37,17,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(37,34,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(37,54,0), new Vector3Int(16,16,3));


		split.addBox(new Vector3Int(54,0,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(54,17,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(54,34,0), new Vector3Int(16,16,3));
		split.addBox(new Vector3Int(54,54,0), new Vector3Int(16,16,3));
		/**/

		VoxelStruct[] voxels = split.doIt ();
		for (int i = 0; i < voxels.Length; ++i) {
			FileStream sw = new FileStream ("cool"+i+".vox", FileMode.Create, FileAccess.Write);

			System.IO.BinaryWriter bw = new System.IO.BinaryWriter (sw); 
			//voxels [i].normal ;
			MagicaVoxelFormater.WriteToBinary (voxels[i], bw);
			sw.Close ();
		}
		//VoxelStruct vs2 = splice.spliceAll ();


	}
	[Test]
	public void JoinVoxelTest(){

		FileStream sr2 = new FileStream (".//Assets//Voxel//temp.vox", FileMode.OpenOrCreate, FileAccess.Read);
		System.IO.BinaryReader br2 = new System.IO.BinaryReader (sr2); 
		VoxelStruct vs = MagicaVoxelFormater.ReadFromBinary (br2).structure;
		Debug.Log (MagicaVoxelFormater.GetMd5(vs));



	}

	[Test]
	public void VoxelMapMakerTest(){/*
		VoxelMapMaker vmm = Component.FindObjectOfType<VoxelMapMaker> ();
		VoxelStruct map = vmm.building ();
		FileStream sw = new FileStream ("map.vox", FileMode.Create, FileAccess.Write);
		System.IO.BinaryWriter bw = new System.IO.BinaryWriter (sw); 
		VoxelFormater.WriteToMagicaVoxel (map, bw);
		sw.Close ();

		Debug.Log (vmm);*/
	}
    [Test]
	public void VoxelFormaterTest()
    {
        //Arrange
       // var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
       // var newGameObjectName = "My game object";
        //gameObject.name = newGameObjectName;

		//VoxelWriter writer = gameObject.GetComponent<VoxelWriter> ();
		//VoxelFormater formater = gameObject.GetComponent<VoxelFormater> ();

		FileStream sr = new FileStream (".//Assets//Voxel//chr_cop2.bytes", FileMode.OpenOrCreate, FileAccess.Read);


		System.IO.BinaryReader br = new System.IO.BinaryReader (sr); 


		MagicaVoxel magic = MagicaVoxelFormater.ReadFromBinary (br);


		FileStream sw = new FileStream ("fly2.vox", FileMode.Create, FileAccess.Write);

		System.IO.BinaryWriter bw = new System.IO.BinaryWriter (sw); 
		MagicaVoxelFormater.WriteToBinary (magic.structure, bw);


		sw.Close ();
		sr.Close ();

		FileStream sr2 = new FileStream ("fly2.vox", FileMode.OpenOrCreate, FileAccess.Read);

	
		System.IO.BinaryReader br2 = new System.IO.BinaryReader (sr2); 

		MagicaVoxel magic2 = MagicaVoxelFormater.ReadFromBinary (br2);

		Assert.AreEqual(magic.main.name, magic2.main.name);
		Assert.AreEqual(magic.main.size, magic2.main.size);
		Assert.AreEqual(magic.main.chunks, magic2.main.chunks);


		Assert.AreEqual(magic.size.box, magic2.size.box);
		Assert.AreEqual(magic.size.name, magic2.size.name);
		Assert.AreEqual(magic.size.size, magic2.size.size);
		Assert.AreEqual(magic.size.chunks, magic2.size.chunks);


		Assert.AreEqual(magic.rgba.palette.Length, magic2.rgba.palette.Length);
		for (int i = 0; i < magic.rgba.palette.Length; ++i) {
			Assert.AreEqual(magic.rgba.palette[i], magic2.rgba.palette[i]);
		}

//		Debug.Log (vs2.rgba.palette.Length);
		Assert.AreEqual(magic.rgba.name, magic2.rgba.name);
		Assert.AreEqual(magic.rgba.size, magic2.rgba.size);
		Assert.AreEqual(magic.rgba.chunks, magic2.rgba.chunks);

		sr2.Close ();

		for (int i = 0; i < MagicaVoxelFormater.palette_.Length; ++i) {
			
			ushort s = MagicaVoxelFormater.palette_ [i];
			Color c = MagicaVoxelFormater.Short2Color (s);
			ushort s2 = MagicaVoxelFormater.Color2Short (c);
			Color c2 = MagicaVoxelFormater.Short2Color (s2);
			Assert.AreEqual(s, s2);
			Assert.AreEqual(c, c2);

		}


		//Debug.Log ();
		Assert.AreEqual(magic.structure.count, magic2.structure.count);

//		Debug.Log (vs2.datas.Length);
		for (int i = 0; i < magic.structure.count; ++i) {
			Assert.AreEqual(magic.structure.getData(i).color, magic2.structure.getData(i).color);
			Assert.AreEqual(magic.structure.getData(i).position.x, magic2.structure.getData(i).position.x);
			Assert.AreEqual(magic.structure.getData(i).position.y, magic2.structure.getData(i).position.y);
			Assert.AreEqual(magic.structure.getData(i).position.z, magic2.structure.getData(i).position.z);
		}


		Assert.AreEqual (magic.structure.count, magic.structure.count);	

    }
}
