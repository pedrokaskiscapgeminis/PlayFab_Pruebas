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
using PlayFab.CloudScriptModels;

public class PlayFabLogin : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    
   
    private EntityKey groupAdminEntity;

    [System.Serializable]
    public class PlayerData
    {
        public string getPlayerUsername;
    }


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
            RequireBothUsernameAndEmail = true
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
        messageText.text = result.ToJson();
        Debug.Log(result.ToJson());
        var jsonConverter = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        groupAdminEntity = jsonConverter.DeserializeObject<PlayFab.GroupsModels.EntityKey>(jsonConverter.SerializeObject(playerEntity));
        Debug.Log(groupAdminEntity.ToJson());

        
    

    var AddMem = new ExecuteCloudScriptRequest()
        {
            FunctionName = "addMember",
            FunctionParameter = new
            {
                GroupId = "77569033BA83F38B",
            },
            //GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(AddMem, OnAddMemberSuccess, OnAddMemberFailure);



    var GetNa = new ExecuteCloudScriptRequest()
        {
            FunctionName = "getPlayerAccountInfo"
        };

        PlayFabClientAPI.ExecuteCloudScript(GetNa, OnExecureSuccess, OnError);
    }

    private void OnAddMemberSuccess(PlayFab.ClientModels.ExecuteCloudScriptResult result)
    {
        Debug.Log("Member added to group successfully." + result.ToJson());
    }

    private void OnAddMemberFailure(PlayFabError error)
    {
        Debug.LogError("Error adding member to group: " + error.ErrorMessage);
    }



    //ESTA SI QUE VA PORQUEEEEEEEE!!!!!!
    /*var request = new ExecuteCloudScriptRequest()
    {
        FunctionName = "hello",
    };
    PlayFabClientAPI.ExecuteCloudScript(request, OnExecureSuccess, OnError);*/

   
void OnExecureSuccess(PlayFab.ClientModels.ExecuteCloudScriptResult result)
    {

        //Debug.Log(result.FunctionResult);



        string jsonString = result.FunctionResult.ToString();

        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonString);

        string username = playerData.getPlayerUsername;

        Debug.Log(username); // output: "prueba1"


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