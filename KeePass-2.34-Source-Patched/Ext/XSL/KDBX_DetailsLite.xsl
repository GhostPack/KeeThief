<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
<xsl:apply-templates select="KeePassFile" />
</xsl:template>

<xsl:template match="KeePassFile">
<html>
<head>
<xsl:apply-templates select="Meta" />
<link rel="stylesheet" type="text/css" href="KDBX_Styles.css" />
</head>
<body>
<xsl:apply-templates select="Root" />
</body>
</html>
</xsl:template>

<xsl:template match="Meta">
<title><xsl:value-of select="DatabaseName" /></title>
</xsl:template>

<xsl:template match="Root">
<xsl:for-each select="Group">
<xsl:apply-templates select="." />
</xsl:for-each>
</xsl:template>

<xsl:template match="Group">
<xsl:for-each select="Entry">
<xsl:apply-templates select="." />
</xsl:for-each>
<xsl:for-each select="Group">
<xsl:apply-templates select="." />
</xsl:for-each>
</xsl:template>

<xsl:template match="Entry">

<table class="tablebox">
<tr><td class="smallboxtitle">
<xsl:for-each select="String[Key='Title']"><xsl:value-of select="Value" /></xsl:for-each>
</td></tr>

<tr><td class="boxcontent">

<i>Title: </i><xsl:for-each select="String[Key='Title']"><xsl:value-of select="Value" /></xsl:for-each><br />
<i>User Name: </i><xsl:for-each select="String[Key='UserName']"><xsl:value-of select="Value" /></xsl:for-each><br />
<i>Password: </i><xsl:for-each select="String[Key='Password']"><xsl:value-of select="Value" /></xsl:for-each><br />

<i>URL: </i>
<xsl:for-each select="String[Key='URL']">
<xsl:element name= "a">
<xsl:attribute name="href">
<xsl:value-of select="Value" />
</xsl:attribute>
<xsl:value-of select="Value" />
</xsl:element>
</xsl:for-each>
<br />

<i>Notes: </i><xsl:for-each select="String[Key='Notes']"><xsl:value-of select="Value" /></xsl:for-each><br />

<xsl:for-each select="String">

<xsl:if test="Key != 'Title'">
<xsl:if test="Key != 'UserName'">
<xsl:if test="Key != 'Password'">
<xsl:if test="Key != 'URL'">
<xsl:if test="Key != 'Notes'">
<i><xsl:value-of select="Key" />: </i>
<xsl:value-of select="Value" /><br />
</xsl:if>
</xsl:if>
</xsl:if>
</xsl:if>
</xsl:if>
</xsl:for-each>

<xsl:if test="Times/Expires = 'True'">
<i>Expires: </i><xsl:value-of select="Times/ExpiryTime" />
</xsl:if>

</td></tr></table><br />

</xsl:template>

</xsl:stylesheet>
