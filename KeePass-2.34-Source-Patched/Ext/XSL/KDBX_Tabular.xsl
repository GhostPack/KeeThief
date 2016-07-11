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
<table class="tablebox">
<tr>
<td class="smallboxtitle" width="20%" nowrap="nowrap" align="center"><b><i>Title</i></b></td>
<td class="smallboxtitle" width="20%" nowrap="nowrap" align="center"><b><i>User Name</i></b></td>
<td class="smallboxtitle" width="20%" nowrap="nowrap" align="center"><b><i>Password</i></b></td>
<td class="smallboxtitle" width="20%" nowrap="nowrap" align="center"><b><i>URL</i></b></td>
<td class="smallboxtitle" width="20%" nowrap="nowrap" align="center"><b><i>Notes</i></b></td>
</tr>
<xsl:for-each select="Group">
<xsl:apply-templates select="." />
</xsl:for-each>
</table>
</xsl:template>

<xsl:template match="Group">
<tr>
<td class="boxcontent"><big><b><xsl:value-of select="Name" /></b></big></td>
<td class="boxcontent"> </td>
<td class="boxcontent"> </td>
<td class="boxcontent"> </td>
<td class="boxcontent"> </td>
</tr>

<xsl:for-each select="Entry">
<xsl:apply-templates select="." />
</xsl:for-each>

<xsl:for-each select="Group">
<xsl:apply-templates select="." />
</xsl:for-each>

</xsl:template>

<xsl:template match="Entry">
<tr>
<td class="boxcontent"><xsl:for-each select="String[Key='Title']"><xsl:value-of select="Value" /></xsl:for-each></td>
<td class="boxcontent"><xsl:for-each select="String[Key='UserName']"><xsl:value-of select="Value" /></xsl:for-each></td>
<td class="boxcontent"><xsl:for-each select="String[Key='Password']"><xsl:value-of select="Value" /></xsl:for-each></td>

<td class="boxcontent">
<xsl:element name= "a">
<xsl:attribute name="href">
<xsl:for-each select="String[Key='URL']"><xsl:value-of select="Value" /></xsl:for-each>
</xsl:attribute>
<xsl:for-each select="String[Key='URL']"><xsl:value-of select="Value" /></xsl:for-each>
</xsl:element>
</td>

<td class="boxcontent"><xsl:for-each select="String[Key='Notes']"><xsl:value-of select="Value" /></xsl:for-each></td>
</tr>
</xsl:template>

</xsl:stylesheet>
