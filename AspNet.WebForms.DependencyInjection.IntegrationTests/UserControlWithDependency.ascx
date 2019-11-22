<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserControlWithDependency.ascx.cs" Inherits="PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests.UserControlWithDependency" %>

<p>Doggo from the UserControlWithDependency user control</p>
<!-- Use the DogManager/DogRepository -->
<asp:GridView runat="server" ID="doggoList" ItemType="PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests.Dog" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="Name" />
        <asp:BoundField DataField="TatooNumber" />
    </Columns>
</asp:GridView>
