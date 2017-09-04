#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>
#include <assert.h>
#include <exception>

// EnemySpawner
struct EnemySpawner_t2483298528;
// System.Collections.IEnumerator
struct IEnumerator_t3464575207;
// System.Int32[]
struct Int32U5BU5D_t3230847821;

#include "codegen/il2cpp-codegen.h"

// System.Void EnemySpawner::.ctor()
extern "C"  void EnemySpawner__ctor_m648727995 (EnemySpawner_t2483298528 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void EnemySpawner::Start()
extern "C"  void EnemySpawner_Start_m3890833083 (EnemySpawner_t2483298528 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void EnemySpawner::Update()
extern "C"  void EnemySpawner_Update_m362593458 (EnemySpawner_t2483298528 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Collections.IEnumerator EnemySpawner::SpawnEnemyLevel(System.Int32[])
extern "C"  Il2CppObject * EnemySpawner_SpawnEnemyLevel_m2040548535 (EnemySpawner_t2483298528 * __this, Int32U5BU5D_t3230847821* ___types0, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void EnemySpawner::SpawnHorizontal(System.Int32)
extern "C"  void EnemySpawner_SpawnHorizontal_m2852622441 (EnemySpawner_t2483298528 * __this, int32_t ___numEnemy0, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Collections.IEnumerator EnemySpawner::SpawnSlash(System.Boolean,System.Int32)
extern "C"  Il2CppObject * EnemySpawner_SpawnSlash_m1286335533 (EnemySpawner_t2483298528 * __this, bool ___ForB0, int32_t ___numEnemy1, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Collections.IEnumerator EnemySpawner::SpawnCaret(System.Int32)
extern "C"  Il2CppObject * EnemySpawner_SpawnCaret_m1946091786 (EnemySpawner_t2483298528 * __this, int32_t ___numEnemy0, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void EnemySpawner::genSpike(System.Single,System.Single,System.Single)
extern "C"  void EnemySpawner_genSpike_m2658595318 (EnemySpawner_t2483298528 * __this, float ___x0, float ___y1, float ___z2, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void EnemySpawner::genBird(System.Single,System.Single,System.Single)
extern "C"  void EnemySpawner_genBird_m3172914067 (EnemySpawner_t2483298528 * __this, float ___x0, float ___y1, float ___z2, const MethodInfo* method) IL2CPP_METHOD_ATTR;
