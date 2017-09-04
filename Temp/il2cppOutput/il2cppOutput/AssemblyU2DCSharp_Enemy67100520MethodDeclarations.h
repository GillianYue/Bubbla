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

// Enemy
struct Enemy_t67100520;
// UnityEngine.Collider
struct Collider_t2939674232;

#include "codegen/il2cpp-codegen.h"
#include "UnityEngine_UnityEngine_Collider2939674232.h"

// System.Void Enemy::.ctor()
extern "C"  void Enemy__ctor_m1781972739 (Enemy_t67100520 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void Enemy::Start()
extern "C"  void Enemy_Start_m729110531 (Enemy_t67100520 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void Enemy::Update()
extern "C"  void Enemy_Update_m1133442154 (Enemy_t67100520 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Int32 Enemy::getLife()
extern "C"  int32_t Enemy_getLife_m1093666113 (Enemy_t67100520 * __this, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void Enemy::setValues(System.Int32,System.Int32)
extern "C"  void Enemy_setValues_m360617825 (Enemy_t67100520 * __this, int32_t ___LIFE0, int32_t ___ATTACK1, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void Enemy::damage(System.Int32)
extern "C"  void Enemy_damage_m331865793 (Enemy_t67100520 * __this, int32_t ___damage0, const MethodInfo* method) IL2CPP_METHOD_ATTR;
// System.Void Enemy::OnTriggerEnter(UnityEngine.Collider)
extern "C"  void Enemy_OnTriggerEnter_m87527253 (Enemy_t67100520 * __this, Collider_t2939674232 * ___other0, const MethodInfo* method) IL2CPP_METHOD_ATTR;
