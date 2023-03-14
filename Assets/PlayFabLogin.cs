using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.PackageManager.Requests;
using PlayFab.GroupsModels;
using EntityKey = PlayFab.GroupsModels.EntityKey;
using EmptyResponse = PlayFab.GroupsModels.EmptyResponse;
using PlayFab.Internal;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEditor.PackageManager;
using PlayFab.AuthenticationModels;

public class PlayFabLogin : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    
   
    private EntityKey groupAdminEntity;

    public void RegisterButton()
    {
        if(passwordInput.text.Length < 6) {
            messageText.text = "Password too Short!";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSucess,OnError);
    }
    void OnRegisterSucess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Registered and logged in!";
    }
    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }
    void OnLoginSuccess(LoginResult result)
    {
        PlayFab.ClientModels.EntityKey playerEntity;
        playerEntity = result.EntityToken.Entity;
        messageText.text = "Logged in!";
        var jsonConverter = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        groupAdminEntity = jsonConverter.DeserializeObject<PlayFab.GroupsModels.EntityKey>(jsonConverter.SerializeObject(playerEntity));
        Debug.Log(groupAdminEntity.ToJson());

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "addMembers",
            FunctionParameter = new 
            {
                GroupId = "77569033BA83F38B",
                MemberIDs = groupAdminEntity.Id.ToString()
            },
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnExecureSuccess, OnError);
    }

    private void OnExecureSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log(result);
        Debug.Log(result.FunctionResult);
    }





    // var request = new ApplyToGroupRequest { Entity = groupAdminEntity, Group = new EntityKey{Id = groupId}, AutoAcceptOutstandingInvite = true };
    // PlayFabGroupsAPI.ApplyToGroup(request, OnApply, OnError);




    /*public void OnApply(ApplyToGroupResponse response)
    {
        var prevRequest = (ApplyToGroupRequest)response.Request;

        // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
        var request = new AcceptGroupApplicationRequest { Entity = prevRequest.Entity,Group = prevRequest.Group };
        Debug.Log("Peta aqui");
        PlayFabGroupsAPI.AcceptGroupApplication(request, OnAcceptApplication, OnError);
    }
    public void OnAcceptApplication(EmptyResponse response)
    {
        var prevRequest = (AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
    }*/

    public void ResetPassword()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "CB001",
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset mail sent!";
    }
   
    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }
   
}