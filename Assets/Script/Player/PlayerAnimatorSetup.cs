#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility: crea el AnimatorController del astronauta con los
/// estados, parámetros y transiciones apropiadas para los sprites existentes.
///
/// Uso: menú superior de Unity → SpaceDungeon → Setup → Create Player Animator
/// </summary>
public static class PlayerAnimatorSetup
{
    private const string CONTROLLER_PATH = "Assets/Animations/Player_Animator.controller";

    [MenuItem("SpaceDungeon/Setup/Create Player Animator Controller")]
    public static AnimatorController CreatePlayerAnimator()
    {
        // ── Crear o sobreescribir el controller ──────────────────────────────
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(CONTROLLER_PATH);

        // ── Parámetros ───────────────────────────────────────────────────────
        controller.AddParameter("Speed",           AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded",      AnimatorControllerParameterType.Bool);
        controller.AddParameter("VerticalVelocity", AnimatorControllerParameterType.Float);

        // ── Capa base ────────────────────────────────────────────────────────
        AnimatorStateMachine rootSM = controller.layers[0].stateMachine;

        // ── Estados ──────────────────────────────────────────────────────────
        AnimatorState stateIdle = rootSM.AddState("Idle",         new Vector3(250, 0,   0));
        AnimatorState stateRun  = rootSM.AddState("Run",          new Vector3(250, 100, 0));
        AnimatorState stateJump = rootSM.AddState("Jump_Rise",    new Vector3(500, 50,  0));
        AnimatorState stateFall = rootSM.AddState("Fall",         new Vector3(500, 150, 0));

        // Asignar clips de animación (se buscan por nombre de sprite sheet)
        stateIdle.motion = FindClipOrLogWarning("idle 112x26");
        stateRun .motion = FindClipOrLogWarning("move 112x26");
        stateJump.motion = FindClipOrLogWarning("fire dash 112x26");
        stateFall.motion = FindClipOrLogWarning("damaged 112x26");

        // Estado inicial: Idle
        rootSM.defaultState = stateIdle;

        // ── Transiciones ─────────────────────────────────────────────────────

        // Idle → Run (Speed > 0.05 y en suelo)
        AnimatorStateTransition idleToRun = stateIdle.AddTransition(stateRun);
        idleToRun.AddCondition(AnimatorConditionMode.Greater, 0.05f, "Speed");
        idleToRun.AddCondition(AnimatorConditionMode.If,     0f,    "IsGrounded");
        idleToRun.duration             = 0.05f;
        idleToRun.hasExitTime          = false;

        // Run → Idle (Speed ≤ 0.05 y en suelo)
        AnimatorStateTransition runToIdle = stateRun.AddTransition(stateIdle);
        runToIdle.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed");
        runToIdle.AddCondition(AnimatorConditionMode.If,   0f,    "IsGrounded");
        runToIdle.duration    = 0.05f;
        runToIdle.hasExitTime = false;

        // Idle → Jump (VerticalVelocity > 0.1 y NO IsGrounded)
        AnimatorStateTransition idleToJump = stateIdle.AddTransition(stateJump);
        idleToJump.AddCondition(AnimatorConditionMode.Greater, 0.1f, "VerticalVelocity");
        idleToJump.AddCondition(AnimatorConditionMode.IfNot,   0f,   "IsGrounded");
        idleToJump.duration    = 0.05f;
        idleToJump.hasExitTime = false;

        // Run → Jump (VerticalVelocity > 0.1 y NO IsGrounded)
        AnimatorStateTransition runToJump = stateRun.AddTransition(stateJump);
        runToJump.AddCondition(AnimatorConditionMode.Greater, 0.1f, "VerticalVelocity");
        runToJump.AddCondition(AnimatorConditionMode.IfNot,   0f,   "IsGrounded");
        runToJump.duration    = 0.05f;
        runToJump.hasExitTime = false;

        // Jump → Fall (VerticalVelocity ≤ 0)
        AnimatorStateTransition jumpToFall = stateJump.AddTransition(stateFall);
        jumpToFall.AddCondition(AnimatorConditionMode.Less, 0f, "VerticalVelocity");
        jumpToFall.duration    = 0.05f;
        jumpToFall.hasExitTime = false;

        // Fall → Idle (IsGrounded y Speed ≤ 0.05)
        AnimatorStateTransition fallToIdle = stateFall.AddTransition(stateIdle);
        fallToIdle.AddCondition(AnimatorConditionMode.If,   0f,    "IsGrounded");
        fallToIdle.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed");
        fallToIdle.duration    = 0.05f;
        fallToIdle.hasExitTime = false;

        // Fall → Run (IsGrounded y Speed > 0.05)
        AnimatorStateTransition fallToRun = stateFall.AddTransition(stateRun);
        fallToRun.AddCondition(AnimatorConditionMode.If,      0f,    "IsGrounded");
        fallToRun.AddCondition(AnimatorConditionMode.Greater, 0.05f, "Speed");
        fallToRun.duration    = 0.05f;
        fallToRun.hasExitTime = false;

        // ── Guardar asset ────────────────────────────────────────────────────
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SpaceDungeon] AnimatorController creado en: {CONTROLLER_PATH}");
        Selection.activeObject = controller;

        return controller;
    }

    // ─── Helper: busca el primer clip de animación que contiene el nombre ────
    private static AnimationClip FindClipOrLogWarning(string spriteSheetName)
    {
        string[] guids = AssetDatabase.FindAssets($"t:AnimationClip {spriteSheetName}");

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }

        Debug.LogWarning($"[SpaceDungeon] No se encontró clip de animación para '{spriteSheetName}'. " +
                         $"Crea el clip manualmente y asígnalo al estado correspondiente.");
        return null;
    }
}
#endif
