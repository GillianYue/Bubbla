#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>

// UnityEngine.GameObject
struct GameObject_t3674682005;
// UnityEngine.Sprite[]
struct SpriteU5BU5D_t2761310900;

#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"
#include "UnityEngine_UnityEngine_Color4194546905.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// PaintballSpawner
struct  PaintballSpawner_t17610603  : public MonoBehaviour_t667441552
{
public:
	// System.Int32 PaintballSpawner::size
	int32_t ___size_2;
	// UnityEngine.Color PaintballSpawner::color
	Color_t4194546905  ___color_3;
	// System.Single PaintballSpawner::sizeScale
	float ___sizeScale_4;
	// System.Int32 PaintballSpawner::myNumInList
	int32_t ___myNumInList_5;
	// UnityEngine.GameObject PaintballSpawner::explosion
	GameObject_t3674682005 * ___explosion_6;
	// UnityEngine.GameObject PaintballSpawner::absorption
	GameObject_t3674682005 * ___absorption_7;
	// UnityEngine.Sprite[] PaintballSpawner::pbSprites
	SpriteU5BU5D_t2761310900* ___pbSprites_8;
	// UnityEngine.Sprite[] PaintballSpawner::highlights
	SpriteU5BU5D_t2761310900* ___highlights_9;
	// System.Int32 PaintballSpawner::num
	int32_t ___num_10;

public:
	inline static int32_t get_offset_of_size_2() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___size_2)); }
	inline int32_t get_size_2() const { return ___size_2; }
	inline int32_t* get_address_of_size_2() { return &___size_2; }
	inline void set_size_2(int32_t value)
	{
		___size_2 = value;
	}

	inline static int32_t get_offset_of_color_3() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___color_3)); }
	inline Color_t4194546905  get_color_3() const { return ___color_3; }
	inline Color_t4194546905 * get_address_of_color_3() { return &___color_3; }
	inline void set_color_3(Color_t4194546905  value)
	{
		___color_3 = value;
	}

	inline static int32_t get_offset_of_sizeScale_4() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___sizeScale_4)); }
	inline float get_sizeScale_4() const { return ___sizeScale_4; }
	inline float* get_address_of_sizeScale_4() { return &___sizeScale_4; }
	inline void set_sizeScale_4(float value)
	{
		___sizeScale_4 = value;
	}

	inline static int32_t get_offset_of_myNumInList_5() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___myNumInList_5)); }
	inline int32_t get_myNumInList_5() const { return ___myNumInList_5; }
	inline int32_t* get_address_of_myNumInList_5() { return &___myNumInList_5; }
	inline void set_myNumInList_5(int32_t value)
	{
		___myNumInList_5 = value;
	}

	inline static int32_t get_offset_of_explosion_6() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___explosion_6)); }
	inline GameObject_t3674682005 * get_explosion_6() const { return ___explosion_6; }
	inline GameObject_t3674682005 ** get_address_of_explosion_6() { return &___explosion_6; }
	inline void set_explosion_6(GameObject_t3674682005 * value)
	{
		___explosion_6 = value;
		Il2CppCodeGenWriteBarrier(&___explosion_6, value);
	}

	inline static int32_t get_offset_of_absorption_7() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___absorption_7)); }
	inline GameObject_t3674682005 * get_absorption_7() const { return ___absorption_7; }
	inline GameObject_t3674682005 ** get_address_of_absorption_7() { return &___absorption_7; }
	inline void set_absorption_7(GameObject_t3674682005 * value)
	{
		___absorption_7 = value;
		Il2CppCodeGenWriteBarrier(&___absorption_7, value);
	}

	inline static int32_t get_offset_of_pbSprites_8() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___pbSprites_8)); }
	inline SpriteU5BU5D_t2761310900* get_pbSprites_8() const { return ___pbSprites_8; }
	inline SpriteU5BU5D_t2761310900** get_address_of_pbSprites_8() { return &___pbSprites_8; }
	inline void set_pbSprites_8(SpriteU5BU5D_t2761310900* value)
	{
		___pbSprites_8 = value;
		Il2CppCodeGenWriteBarrier(&___pbSprites_8, value);
	}

	inline static int32_t get_offset_of_highlights_9() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___highlights_9)); }
	inline SpriteU5BU5D_t2761310900* get_highlights_9() const { return ___highlights_9; }
	inline SpriteU5BU5D_t2761310900** get_address_of_highlights_9() { return &___highlights_9; }
	inline void set_highlights_9(SpriteU5BU5D_t2761310900* value)
	{
		___highlights_9 = value;
		Il2CppCodeGenWriteBarrier(&___highlights_9, value);
	}

	inline static int32_t get_offset_of_num_10() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___num_10)); }
	inline int32_t get_num_10() const { return ___num_10; }
	inline int32_t* get_address_of_num_10() { return &___num_10; }
	inline void set_num_10(int32_t value)
	{
		___num_10 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
