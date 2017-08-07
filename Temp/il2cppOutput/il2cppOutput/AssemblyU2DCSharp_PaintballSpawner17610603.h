#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>

// UnityEngine.Renderer
struct Renderer_t3076687687;
// UnityEngine.GameObject
struct GameObject_t3674682005;

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
	// UnityEngine.Renderer PaintballSpawner::rend
	Renderer_t3076687687 * ___rend_2;
	// System.Int32 PaintballSpawner::size
	int32_t ___size_3;
	// UnityEngine.Color PaintballSpawner::color
	Color_t4194546905  ___color_4;
	// System.Single PaintballSpawner::sizeScale
	float ___sizeScale_5;
	// System.Int32 PaintballSpawner::myNumInList
	int32_t ___myNumInList_6;
	// UnityEngine.GameObject PaintballSpawner::explosion
	GameObject_t3674682005 * ___explosion_7;
	// UnityEngine.GameObject PaintballSpawner::absorption
	GameObject_t3674682005 * ___absorption_8;

public:
	inline static int32_t get_offset_of_rend_2() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___rend_2)); }
	inline Renderer_t3076687687 * get_rend_2() const { return ___rend_2; }
	inline Renderer_t3076687687 ** get_address_of_rend_2() { return &___rend_2; }
	inline void set_rend_2(Renderer_t3076687687 * value)
	{
		___rend_2 = value;
		Il2CppCodeGenWriteBarrier(&___rend_2, value);
	}

	inline static int32_t get_offset_of_size_3() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___size_3)); }
	inline int32_t get_size_3() const { return ___size_3; }
	inline int32_t* get_address_of_size_3() { return &___size_3; }
	inline void set_size_3(int32_t value)
	{
		___size_3 = value;
	}

	inline static int32_t get_offset_of_color_4() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___color_4)); }
	inline Color_t4194546905  get_color_4() const { return ___color_4; }
	inline Color_t4194546905 * get_address_of_color_4() { return &___color_4; }
	inline void set_color_4(Color_t4194546905  value)
	{
		___color_4 = value;
	}

	inline static int32_t get_offset_of_sizeScale_5() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___sizeScale_5)); }
	inline float get_sizeScale_5() const { return ___sizeScale_5; }
	inline float* get_address_of_sizeScale_5() { return &___sizeScale_5; }
	inline void set_sizeScale_5(float value)
	{
		___sizeScale_5 = value;
	}

	inline static int32_t get_offset_of_myNumInList_6() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___myNumInList_6)); }
	inline int32_t get_myNumInList_6() const { return ___myNumInList_6; }
	inline int32_t* get_address_of_myNumInList_6() { return &___myNumInList_6; }
	inline void set_myNumInList_6(int32_t value)
	{
		___myNumInList_6 = value;
	}

	inline static int32_t get_offset_of_explosion_7() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___explosion_7)); }
	inline GameObject_t3674682005 * get_explosion_7() const { return ___explosion_7; }
	inline GameObject_t3674682005 ** get_address_of_explosion_7() { return &___explosion_7; }
	inline void set_explosion_7(GameObject_t3674682005 * value)
	{
		___explosion_7 = value;
		Il2CppCodeGenWriteBarrier(&___explosion_7, value);
	}

	inline static int32_t get_offset_of_absorption_8() { return static_cast<int32_t>(offsetof(PaintballSpawner_t17610603, ___absorption_8)); }
	inline GameObject_t3674682005 * get_absorption_8() const { return ___absorption_8; }
	inline GameObject_t3674682005 ** get_address_of_absorption_8() { return &___absorption_8; }
	inline void set_absorption_8(GameObject_t3674682005 * value)
	{
		___absorption_8 = value;
		Il2CppCodeGenWriteBarrier(&___absorption_8, value);
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
